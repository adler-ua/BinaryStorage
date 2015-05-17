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

        public void SaveFile(Stream data, out long offset, out long size)
        {
            using (FileStream stream = new FileStream(_path,FileMode.Append))
            {
                offset = stream.Length;
                size = data.Length;
                data.CopyTo(stream);
            }
        }

        public Stream RestoreFile(long offset, long size)
        {
            using (FileStream stream = File.OpenRead(_path))
            {
                byte[] bytes = new byte[size];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(bytes, 0, (int) size);
                //MemoryStream ms = new MemoryStream();
                
                Stream s = new MemoryStream(bytes);
                return s;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
