using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage(configuration.WorkingFolder);
            IPersistentStreamStorage persistentStreamStorage = new FileStreamStorage(configuration.WorkingFolder);
            
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
            long offset, size;
            _streamStorage.SaveFile(data, parameters, out offset, out size);
            Index index = _indexStorage.Add(key, offset, size, parameters);
        }

        public Stream Get(string key)
        {
            Index index = _indexStorage.Get(key);
            return _streamStorage.RestoreFile(index.Offset, index.Size);
        }

        public bool Contains(string key)
        {
            return _indexStorage.ContainsKey(key);
        }

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
