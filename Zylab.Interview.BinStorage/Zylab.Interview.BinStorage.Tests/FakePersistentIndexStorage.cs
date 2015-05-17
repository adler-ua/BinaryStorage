using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    internal class FakePersistentIndexStorage : IPersistentIndexStorage
    {
        private ConcurrentDictionary<string, Index> _fakeStorage = new ConcurrentDictionary<string, Index>();

        public void Save(ConcurrentDictionary<string, Index> items)
        {
            _fakeStorage = items;
        }

        public void Save(Index item)
        {
            _fakeStorage[item.Key] = item;
        }

        public ConcurrentDictionary<string, Index> Restore()
        {
            return _fakeStorage;
        }

        public void Dispose()
        {
            
        }
    }
}
