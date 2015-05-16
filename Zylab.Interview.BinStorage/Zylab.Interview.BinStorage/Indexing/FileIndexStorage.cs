using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class FileIndexStorage : IPersistentIndexStorage
    {
        public void Save(IEnumerable<Index> items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Index> Restore()
        {
            throw new NotImplementedException();
        }
    }
}
