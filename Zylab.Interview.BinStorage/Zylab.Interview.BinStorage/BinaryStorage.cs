using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Zylab.Interview.BinStorage.Compressing;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage {
    public class BinaryStorage : IBinaryStorage
    {
        private readonly StorageConfiguration _configuration;
        private readonly IndexStorage _indexStorage;
        private readonly StreamStorage _streamStorage;
        private readonly ICompressor _compressor;
        private readonly NamedReaderWriterLock _rwLock = new NamedReaderWriterLock();
        
        public BinaryStorage(StorageConfiguration configuration)
        {
            _configuration = configuration;
            
            IPersistentIndexStorage persistentIndexStorage = new FileIndexStorage(_configuration.WorkingFolder);
            IPersistentStreamStorage persistentStreamStorage = new FileStreamStorage(_configuration.WorkingFolder);
            
            _indexStorage = new IndexStorage(persistentIndexStorage);
            _streamStorage = new StreamStorage(persistentStreamStorage);
            _compressor = new Compressor();
        }

        public BinaryStorage(StorageConfiguration configuration, IndexStorage indexStorage, StreamStorage streamStorage, ICompressor compressor)
        {
            _configuration = configuration;
            _indexStorage = indexStorage;
            _streamStorage = streamStorage;
            _compressor = compressor;
        }

        #region IBinaryStorage methods

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

            parameters = FulfillParameters(data, parameters);

            Index duplicating = FindDuplicatingData(parameters);
            if (duplicating != null)
            {
                PutReferenceToExistingData(key, parameters, duplicating);
                return;
            }

            Stream compressedStream = _compressor.CompressIfRequired(data, parameters, _configuration.CompressionThreshold);

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

        public Stream Get(string key)
        {
            KeyValuePair<Index, Stream> index_stream = _rwLock.RunWithReadLock(key, () =>
            {
                Index index = _indexStorage.Get(key);
                return new KeyValuePair<Index, Stream>(index, _streamStorage.RestoreFile(key, index.Hash, index.Offset, index.Size));
            });
            if (index_stream.Key.DecompressOnRestore)
            {
                var decompressed = GZipCompressor.Decompress(index_stream.Value);
                decompressed.Seek(0, SeekOrigin.Begin);
                return decompressed;
            }
            return index_stream.Value;
        }

        public bool Contains(string key)
        {
            return _rwLock.RunWithReadLock(key, () => _indexStorage.ContainsKey(key));
        }

        #endregion

        private static StreamInfo FulfillParameters(Stream data, StreamInfo parameters)
        {
            if (parameters.Hash == null)
            {
                parameters = (StreamInfo) parameters.Clone();
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
            return parameters;
        }

        private void PutReferenceToExistingData(string key, StreamInfo parameters, Index duplicating)
        {
            parameters.DecompressOnRestore = duplicating.DecompressOnRestore;
            if (duplicating.CompressionHash != null)
                parameters.CompressionHash = duplicating.CompressionHash.ToArray();
            _indexStorage.Add(key, duplicating.Offset, duplicating.Size, parameters);
        }

        private Index FindDuplicatingData(StreamInfo info)
        {
            if (!info.Length.HasValue || info.Length.Value == 0) return null;
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

        public void Dispose()
        {
            _indexStorage.Dispose();
            _streamStorage.Dispose();
        }
    }
}
