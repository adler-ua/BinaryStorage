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

        public void SaveFile(Stream data)
        {
            byte[] bytes = new byte[data.Length];
            data.Read(bytes, 0, (int)data.Length);
            _fakeStream.Write(bytes, 0, bytes.Length);
        }

        public Stream RestoreFile(long offset, long size)
        {
            byte[] bytes = new byte[size];
            _fakeStream.Seek(offset, SeekOrigin.Begin);
            _fakeStream.Read(bytes, 0, (int)size);
            Stream s = new MemoryStream(bytes);
            return s;
        }

        public long EvaluateOffset()
        {
            return _fakeStream.Length;
        }

        public void Dispose()
        {
            _fakeStream.Dispose();
        }
    }
}
