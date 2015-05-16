using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class FileStreamStorage : IPersistentStreamStorage
    {
        public void SaveFile(Stream data, out long offset, out long size)
        {
            throw new NotImplementedException();
        }

        public Stream RestoreFile(long offset, long size)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
