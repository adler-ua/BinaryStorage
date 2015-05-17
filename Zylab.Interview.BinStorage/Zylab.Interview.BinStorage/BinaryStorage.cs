using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        private static readonly ReaderWriterLockSlim RwIndexLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim RwStorageLock = new ReaderWriterLockSlim();
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage(_configuration.WorkingFolder);
            IPersistentStreamStorage persistentStreamStorage = new FileStreamStorage(_configuration.WorkingFolder);
            
            RwIndexLock.EnterReadLock();
            try
            {
                _indexStorage = new IndexStorage(persistentIndexStorage);
            }
            finally
            {
                RwIndexLock.ExitReadLock();
            }
            RwStorageLock.EnterReadLock();
            try
            {
                _streamStorage = new StreamStorage(persistentStreamStorage);
            }
            finally
            {
                RwStorageLock.ExitReadLock();
            }
        }

        public BinaryStorage(StorageConfiguration configuration, IndexStorage indexStorage, StreamStorage streamStorage)
        {
            _configuration = configuration;
            _indexStorage = indexStorage;
            _streamStorage = streamStorage;
        }

        public void Add(string key, Stream data, StreamInfo parameters)
        {
            CheckKey(key);
            RwStorageLock.EnterWriteLock();
            try
            {
                long offset = _streamStorage.EvaluateOffset();
                Task t = Task.Factory.StartNew(() =>
                {
                    InsertIndex(key, data, parameters, offset);
                });
                try
                {
                    _streamStorage.SaveFile(data, parameters);
                }
                catch (Exception ex)
                {
                    t.Wait();
                    RevertIndexInsert(key);
                    throw;
                }
                t.Wait();
            }
            finally
            {
                RwStorageLock.ExitWriteLock();
            }
            
        }

        private void RevertIndexInsert(string key)
        {
            RwIndexLock.EnterWriteLock();
            try
            {
                _indexStorage.Remove(key);
            }
            finally
            {
                RwIndexLock.ExitWriteLock();
            }
        }

        private void CheckKey(string key)
        {
            RwIndexLock.EnterReadLock();
            try
            {
                if (_indexStorage.ContainsKey(key))
                {
                    throw new DuplicateKeyException(key);
                }
            }
            finally
            {
                RwIndexLock.ExitReadLock();
            }
        }

        private void InsertIndex(string key, Stream data, StreamInfo parameters, long offset)
        {
            RwIndexLock.EnterWriteLock();
            try
            {
                Index index = _indexStorage.Add(key, offset, data.Length, parameters);
            }
            finally
            {
                RwIndexLock.ExitWriteLock();
            }
        }

        public Stream Get(string key)
        {
            RwIndexLock.EnterReadLock();
            try
            {
                Index index = _indexStorage.Get(key);
                return _streamStorage.RestoreFile(index.Offset, index.Size);
            }
            finally
            {
                RwIndexLock.ExitReadLock();
            }
        }

        public bool Contains(string key)
        {
            RwIndexLock.EnterReadLock();
            try
            {
                return _indexStorage.ContainsKey(key);
            }
            finally
            {
                RwIndexLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
