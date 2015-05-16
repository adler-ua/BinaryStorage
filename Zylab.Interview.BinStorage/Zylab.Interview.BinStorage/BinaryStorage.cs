using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private Dictionary<string, Stream> _fileStorage = new Dictionary<string, Stream>();
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage();
            _indexStorage = new IndexStorage(persistentIndexStorage);
        }

        public BinaryStorage(StorageConfiguration configuration, IndexStorage indexStorage)
        {
            _configuration = configuration;
            _indexStorage = indexStorage;
        }

        public void Add(string key, Stream data, StreamInfo parameters) {
            Index index = _indexStorage.Add(key, 0, 0, parameters);
            _fileStorage[index.Key] = data;
        }

        public Stream Get(string key)
        {
            Index index = _indexStorage.Get(key);
            return _fileStorage[index.Key];
        }

        public bool Contains(string key)
        {
            return _indexStorage.ContainsKey(key);
        }

        public void Dispose()
        {
            _fileStorage = null;
        }
    }
}
