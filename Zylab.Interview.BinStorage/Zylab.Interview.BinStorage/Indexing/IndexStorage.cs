﻿using System;
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
            _cache = new Dictionary<string, Index>();
            _persistentStorage = persistentStorage;
        }

        public Index Add(string key, int offset, int size, StreamInfo info)
        {
            Index index = new Index(key, offset, size, info);
            _cache.Add(key, index);
            _persistentStorage.Save(_cache.Values);
            return index;
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public Index Get(string key)
        {
            return _cache[key];
        }

        public bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }
    }

}