using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        
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
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                _rwLock.EnterWriteLock();
                try
                {
                    long offset, size;
                    _streamStorage.SaveFile(data, parameters, out offset, out size);
                    Index index = _indexStorage.Add(key, offset, size, parameters);
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        public Stream Get(string key)
        {
            _rwLock.EnterReadLock();
            try
            {
                Index index = _indexStorage.Get(key);
                return _streamStorage.RestoreFile(index.Offset, index.Size);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public bool Contains(string key)
        {
            _rwLock.EnterReadLock();
            try
            {
                return _indexStorage.ContainsKey(key);
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
