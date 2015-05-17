using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class FileStreamStorage : IPersistentStreamStorage
    {
        private readonly string _directory;
        private readonly string _path;
        private const string StreamStorageFileName = "storage.bin";

        public FileStreamStorage(string directory)
        {
            _directory = directory;
            _path = Path.Combine(_directory, StreamStorageFileName);
        }

        public void SaveFile(Stream data)
        {
            using (FileStream stream = new FileStream(_path,FileMode.Append))
            {
                byte[] bytes = new byte[data.Length];
                data.Read(bytes, 0, (int) data.Length);
                long size = data.Length;
                stream.Write(bytes, 0, (int)size);
            }
        }

        public Stream RestoreFile(long offset, long size)
        {
            using (FileStream stream = File.OpenRead(_path))
            {
                byte[] bytes = new byte[size];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(bytes, 0, (int) size);
                Stream s = new MemoryStream(bytes);
                return s;
            }
        }

        public long EvaluateOffset()
        {
            if (!File.Exists(_path))
            {
                return 0;
            }
            using (FileStream stream = new FileStream(_path, FileMode.Open))
            {
                return stream.Length;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
