using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    internal class FakePersistentIndexStorage : IPersistentIndexStorage
    {
        private Dictionary<string, Index> _fakeStorage;

        public void Save(Dictionary<string, Index> items)
        {
            _fakeStorage = items;
        }

        public Dictionary<string, Index> Restore()
        {
            return _fakeStorage;
        }

        public void Dispose()
        {
            
        }
    }
}
