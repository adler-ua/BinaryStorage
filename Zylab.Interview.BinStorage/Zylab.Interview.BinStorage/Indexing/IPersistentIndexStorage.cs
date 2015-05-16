using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public interface IPersistentIndexStorage
    {
        void Save(Dictionary<string, Index> items);

        Dictionary<string, Index> Restore();

        void Dispose();
    }
}
