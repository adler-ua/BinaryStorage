using System;
using System.IO;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public interface IPersistentStreamStorage : IDisposable
    {
        void SaveFile(Stream data, out long offset, out long size);

        Stream OpenReadStorageStream();
    }
}
