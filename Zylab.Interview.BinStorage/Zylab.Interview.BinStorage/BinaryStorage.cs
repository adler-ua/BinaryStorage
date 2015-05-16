using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {

        private Dictionary<string, Stream> _storage = new Dictionary<string, Stream>();

        public BinaryStorage(StorageConfiguration configuration) {

        }

        public void Add(string key, Stream data, StreamInfo parameters) {
            _storage.Add(key,data);
        }

        public Stream Get(string key)
        {
            return _storage[key];
        }

        public bool Contains(string key)
        {
            return _storage.ContainsKey(key);
        }

        public void Dispose()
        {
            _storage = null;
        }
    }
}
