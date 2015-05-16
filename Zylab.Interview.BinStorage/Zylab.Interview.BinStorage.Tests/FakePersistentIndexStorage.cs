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
        private IEnumerable<Index> _fakeStorage;

        public void Save(IEnumerable<Index> items)
        {
            _fakeStorage = items;
        }

        public IEnumerable<Index> Restore()
        {
            return _fakeStorage;
        }

        public void Dispose()
        {
            
        }
    }
}
