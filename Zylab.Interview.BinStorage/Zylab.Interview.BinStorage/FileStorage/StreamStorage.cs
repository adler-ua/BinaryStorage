using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Newtonsoft.Json.Converters;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class StreamStorage : IDisposable
    {
        private readonly IPersistentStreamStorage _persistentStreamStorage;
        private readonly MemoryCache _cache = new MemoryCache("Storage");

        public StreamStorage(IPersistentStreamStorage persistentStreamStorage)
        {
            _persistentStreamStorage = persistentStreamStorage;
        }

        public void SaveFile(string key, Stream data, StreamInfo streamInfo, out long offset, out long size)
        {
            _persistentStreamStorage.SaveFile(data, out offset, out size);
            //string.Join("", streamInfo.Hash);
            //_cache.Set(key, data, new CacheItemPolicy());
        }

        public Stream RestoreFile(string key, byte[] hash, long offset, long size)
        {
            //string cacheKey = string.Join("", hash);
            //if (_cache.Contains(cacheKey))
            //{
            //    Stream cachedStream = (Stream)_cache.Get(cacheKey);
            //    cachedStream.Seek(0, SeekOrigin.Begin);
            //    Console.WriteLine("Returning cached for file: " + key);
            //    return cachedStream;
            //}
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
            //Console.WriteLine("Caching file: " + key);
            //_cache.Set(cacheKey, destination, new CacheItemPolicy());
            return destination;
        }

        public void Dispose()
        {
            _persistentStreamStorage.Dispose();
        }
    }
}
