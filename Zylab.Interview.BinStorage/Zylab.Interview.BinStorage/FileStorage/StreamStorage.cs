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

            
            Stream destination = new MemoryStream();
            byte[] buffer;
            using (Stream source = _persistentStreamStorage.OpenReadStorageStream())
            {
                source.Seek(offset, SeekOrigin.Begin);
                buffer = new byte[size];
                source.Read(buffer, 0, (int)size);
                destination.Write(buffer, 0, (int)size);
                destination.Seek(0, SeekOrigin.Begin);
            }
            CacheData(buffer, cacheKey);
            return destination;
        }

        private void CacheData(byte[] data, string cacheKey)
        {
            _cache.Set(cacheKey, data, new CacheItemPolicy());
        }

        public void Dispose()
        {
            _cache.Dispose();
            _persistentStreamStorage.Dispose();
        }
    }
}
