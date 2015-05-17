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
            const int BUFFER_SIZE = 4096;
            byte[] buffer = new byte[4096];

            Stream destination = new MemoryStream();
            using (Stream source = _persistentStreamStorage.OpenReadStorageStream())
            {
                source.Seek(offset, SeekOrigin.Begin);
                int bytesToRead = (int)Math.Min(BUFFER_SIZE, size);
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, bytesToRead)) > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                    size -= bytesRead;
                    bytesToRead = (int)Math.Min(BUFFER_SIZE, size);
                    if (bytesToRead <= 0)
                    {
                        destination.Seek(0, SeekOrigin.Begin);
                        break;
                    }
                }
            }
            return destination;
        }

        public void Dispose()
        {
            _persistentStreamStorage.Dispose();
        }
    }
}
