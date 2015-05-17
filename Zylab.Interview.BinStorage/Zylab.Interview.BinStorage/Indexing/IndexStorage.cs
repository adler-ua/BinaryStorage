using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class IndexStorage
    {
        private Dictionary<string, Index> _cache;
        private IPersistentIndexStorage _persistentStorage;
        private readonly object _locker = new object();
        
        public IndexStorage(IPersistentIndexStorage persistentStorage)
        {
            _persistentStorage = persistentStorage;
            _cache = _persistentStorage.Restore();
        }

        private int _cacheVersion;
        private int _savedVersion;
        
        public Index Add(string key, long offset, long size, StreamInfo info)
        {
            Index index = new Index(key, offset, size, info);
            _cache.Add(key, index);
            _cacheVersion++;
            Task.Factory.StartNew(() =>
            {
                lock (_locker)
                {
                    if (_cacheVersion > _savedVersion)
                    {
                        _persistentStorage.Save(_cache);
                        _savedVersion = _cacheVersion;
                    }
                }
            });
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

        public void Dispose()
        {
            lock (_locker)
            {
                _persistentStorage.Dispose();
            }
        }
    }

}
