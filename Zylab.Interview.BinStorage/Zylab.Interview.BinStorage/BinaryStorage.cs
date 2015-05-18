using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json.Converters;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        private readonly NamedReaderWriterLock _rwLock = new NamedReaderWriterLock();
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage(_configuration.WorkingFolder);
            IPersistentStreamStorage persistentStreamStorage = new FileStreamStorage(_configuration.WorkingFolder);
            
            _indexStorage = new IndexStorage(persistentIndexStorage);
            _streamStorage = new StreamStorage(persistentStreamStorage);
        }

        public BinaryStorage(StorageConfiguration configuration, IndexStorage indexStorage, StreamStorage streamStorage)
        {
            _configuration = configuration;
            _indexStorage = indexStorage;
            _streamStorage = streamStorage;
        }

        public void Add(string key, Stream data, StreamInfo parameters)
        {
            _rwLock.RunWithReadLock(key, () =>
            {
                if (_indexStorage.ContainsKey(key))
                {
                    throw new DuplicateKeyException(key);
                }
            });

            ValidateHash(key, data, parameters);

            if (parameters.Hash == null)
            {
                parameters = (StreamInfo)parameters.Clone();
                using (MD5 md5 = MD5.Create())
                {
                    parameters.Hash = md5.ComputeHash(data);
                    data.Seek(0, SeekOrigin.Begin);
                }
            }
            if (!parameters.Length.HasValue)
            {
                parameters.Length = data.Length;
            }

            Stream compressedStream = null;
            if (!parameters.IsCompressed)
            {
                if (_configuration.CompressionThreshold > 0 && parameters.Length > _configuration.CompressionThreshold)
                {
                    compressedStream = Compressing.Compress(data);
                    compressedStream.Seek(0, SeekOrigin.Begin);
                    parameters.DecompressOnRestore = true;
                    parameters.Length = compressedStream.Length;
                    using (MD5 md5 = MD5.Create())
                    {
                        parameters.CompressionHash = md5.ComputeHash(compressedStream);
                        compressedStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            Index duplicating = FindDuplicatingData(parameters);
            if (duplicating != null)
            {
                _indexStorage.Add(key, duplicating.Offset, duplicating.Size, parameters);
                return;
            }

            long offset, size;
            _rwLock.RunWithWriteLock(key, () =>
            {
                Stream streamToSave = data;
                if (compressedStream != null)
                {
                    streamToSave = compressedStream;
                }
                _streamStorage.SaveFile(key, streamToSave, parameters, out offset, out size);
                _indexStorage.Add(key, offset, size, parameters);
            });
        }

        private Index FindDuplicatingData(StreamInfo info)
        {
            long length = info.Length.Value;
            if (length == 0) return null;
            return _indexStorage.FindByHash(info);
        }

        private static void ValidateHash(string key, Stream data, StreamInfo parameters)
        {
            if (parameters.Hash != null)
            {
                using (MD5 md5 = MD5.Create())
                {
                    if (!md5.ComputeHash(data).SequenceEqual(parameters.Hash))
                    {
                        throw new IncorrectHashException(key);
                    }
                }
                data.Seek(0, SeekOrigin.Begin);
            }
        }

        public Stream Get(string key)
        {
            KeyValuePair<Index, Stream> index_stream = _rwLock.RunWithReadLock(key, () =>
            {
                Index index = _indexStorage.Get(key);
                return new KeyValuePair<Index, Stream>(index,_streamStorage.RestoreFile(key, index.Info.Hash, index.Offset, index.Size));
            });
            if (index_stream.Key.Info.DecompressOnRestore == true)
            {
                var decompressed = Compressing.Decompress(index_stream.Value);
                decompressed.Seek(0, SeekOrigin.Begin);
                return decompressed;
            }
            return index_stream.Value;
        }

        public bool Contains(string key)
        {
            return _rwLock.RunWithReadLock(key, () => _indexStorage.ContainsKey(key));
        }

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
