using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class StreamStorage : IDisposable
    {
        private readonly IPersistentStreamStorage _persistentStreamStorage;

        public StreamStorage(IPersistentStreamStorage persistentStreamStorage)
        {
            _persistentStreamStorage = persistentStreamStorage;
        }

        public void SaveFile(Stream data, StreamInfo streamInfo, out long offset, out long size)
        {
            _persistentStreamStorage.SaveFile(data, out offset, out size);
        }

        public Stream RestoreFile(long offset, long size)
        {
            return _persistentStreamStorage.RestoreFile(offset, size);
        }

        public void Dispose()
        {
            _persistentStreamStorage.Dispose();
        }
    }
}
