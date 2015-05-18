using System;
using System.IO;
using System.Runtime.Caching;
using SharpMemoryCache;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class StreamStorage : IDisposable
    {
        private readonly IPersistentStreamStorage _persistentStreamStorage;
        private readonly TrimmingMemoryCache _cache = new TrimmingMemoryCache("Storage");

        public StreamStorage(IPersistentStreamStorage persistentStreamStorage)
        {
            _persistentStreamStorage = persistentStreamStorage;
        }

        public void SaveFile(string key, Stream data, StreamInfo streamInfo, out long offset, out long size)
        {
            _persistentStreamStorage.SaveFile(data, out offset, out size);
        }

        public Stream RestoreFile(string key, byte[] hash, long offset, long size)
        {
            if(size==0)
                return new MemoryStream();
            
            string cacheKey = offset.ToString();
            if (_cache.Contains(cacheKey))
            {
                var cachedBytes = (byte[])_cache.Get(cacheKey);
                MemoryStream cachedStream = new MemoryStream(cachedBytes);
                return cachedStream;
            }

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
            CacheData(destination, cacheKey);
            return destination;
        }

        private void CacheData(Stream stream, string cacheKey)
        {
            var cachedBytes = new byte[stream.Length];
            stream.Read(cachedBytes, 0, (int) stream.Length);
            stream.Seek(0, SeekOrigin.Begin);
            _cache.Set(cacheKey, cachedBytes, new CacheItemPolicy());
        }

        public void Dispose()
        {
            _cache.Dispose();
            _persistentStreamStorage.Dispose();
        }
    }
}
