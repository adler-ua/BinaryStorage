using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public interface IPersistentIndexStorage
    {
        void Save(ConcurrentDictionary<string, Index> items);

        void Save(Index item);

        ConcurrentDictionary<string, Index> Restore();

        void Dispose();
    }
}
