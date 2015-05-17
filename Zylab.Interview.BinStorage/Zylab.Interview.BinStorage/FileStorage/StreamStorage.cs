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

        public void SaveFile(Stream data, StreamInfo streamInfo)
        {
            _persistentStreamStorage.SaveFile(data);
        }

        public Stream RestoreFile(long offset, long size)
        {
            return _persistentStreamStorage.RestoreFile(offset, size);
        }

        public void Dispose()
        {
            _persistentStreamStorage.Dispose();
        }

        public long EvaluateOffset()
        {
            return _persistentStreamStorage.EvaluateOffset();
        }
    }
}
