using System.Collections.Concurrent;

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
