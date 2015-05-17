using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    internal class FakePersistentStreamStorage : IPersistentStreamStorage, IDisposable
    {
        private MemoryStream _fakeStream;

        public FakePersistentStreamStorage()
        {
            _fakeStream = new MemoryStream();
        }

        public void SaveFile(Stream data, out long offset, out long size)
        {
            byte[] bytes = new byte[data.Length];
            data.Read(bytes, 0, (int)data.Length);
            offset = _fakeStream.Length;
            size = data.Length;
            _fakeStream.Write(bytes, 0, bytes.Length);
        }

        public Stream OpenReadStorageStream()
        {
            MemoryStream copy = new MemoryStream();
            _fakeStream.Seek(0, SeekOrigin.Begin);
            _fakeStream.CopyTo(copy);
            _fakeStream.Seek(0, SeekOrigin.End);
            return copy;
        }

        public void Dispose()
        {
            _fakeStream.Dispose();
        }
    }
}
