using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        private readonly ReaderWriterLockSlim _rwIndexLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _rwStorageLock = new ReaderWriterLockSlim();
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage(_configuration.WorkingFolder);
            IPersistentStreamStorage persistentStreamStorage = new FileStreamStorage(_configuration.WorkingFolder);
            
            _indexStorage = new IndexStorage(persistentIndexStorage);
            _streamStorage = new StreamStorage(persistentStreamStorage);
        }

        public BinaryStorage(StorageConfiguration configuration, IndexStorage indexStorage, StreamStorage streamStorage)
        {
            _configuration = configuration;
            _indexStorage = indexStorage;
            _streamStorage = streamStorage;
        }

        public void Add(string key, Stream data, StreamInfo parameters)
        {
            _rwIndexLock.EnterReadLock();
            try
            {
                if (_indexStorage.ContainsKey(key))
                {
                    throw new DuplicateKeyException(key);
                }
            }
            finally
            {
                _rwIndexLock.ExitReadLock();
            }

            // check md5 hash
            if (parameters.Hash != null)
            {
                using (MD5 md5 = MD5.Create())
                {
                    if (!md5.ComputeHash(data).SequenceEqual(parameters.Hash))
                    {
                        throw new IncorrectHashException(key);
                    }
                }
            }
            // --------------

            long offset, size;
            _rwStorageLock.EnterWriteLock();
            try
            {
                _streamStorage.SaveFile(data, parameters, out offset, out size);
            }
            finally
            {
                _rwStorageLock.ExitWriteLock();
            }
            _rwIndexLock.EnterWriteLock();
            try
            {
                Index index = _indexStorage.Add(key, offset, size, parameters);
            }
            finally
            {
                _rwIndexLock.ExitWriteLock();
            }
        }

        public Stream Get(string key)
        {
            _rwIndexLock.EnterReadLock();
            try
            {
                Index index = _indexStorage.Get(key);
                return _streamStorage.RestoreFile(index.Offset, index.Size);
            }
            finally
            {
                _rwIndexLock.ExitReadLock();
            }
        }

        public bool Contains(string key)
        {
            _rwIndexLock.EnterReadLock();
            try
            {
                return _indexStorage.ContainsKey(key);
            }
            finally
            {
                _rwIndexLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
