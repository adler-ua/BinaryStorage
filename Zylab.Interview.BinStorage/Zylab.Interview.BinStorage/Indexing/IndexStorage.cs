using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class IndexStorage
    {
        private Dictionary<string, Index> _cache;
        private IPersistentIndexStorage _persistentStorage;

        public IndexStorage(IPersistentIndexStorage persistentStorage)
        {
            _persistentStorage = persistentStorage;
            _cache = _persistentStorage.Restore();
        }

        public Index Add(string key, long offset, long size, StreamInfo info)
        {
            Index index = new Index(key, offset, size, info);
            _cache.Add(key, index);
            _persistentStorage.Save(index);
            return index;
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Index Get(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                throw new UnknownKeyException(key);
            }
            return _cache[key];
        }

        public bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }

        public void Dispose()
        {
            _persistentStorage.Dispose();
        }
    }

}
