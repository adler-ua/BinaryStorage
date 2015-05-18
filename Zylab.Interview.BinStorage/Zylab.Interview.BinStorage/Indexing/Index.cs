using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Zylab.Interview.BinStorage.JsonUtils;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class Index
    {
        public Index()
        {
            //Info = StreamInfo.Empty;
        }

        public Index(string key, long offset, long size, StreamInfo info)
        {
            if (string.IsNullOrEmpty(key))  throw new ArgumentException("Key should not be empty.","key");
            if (info == null)               throw new ArgumentNullException("info");
            Key = key;
            Offset = offset;
            Size = size;
            //Info = info;
            this.Length = info.Length;
            this.IsCompressed = info.IsCompressed;
            this.DecompressOnRestore = info.DecompressOnRestore;
            this.CompressionHash = info.CompressionHash;
            this.Hash = info.Hash;
        }

        public string Key { get; set; }

        public long Offset { get; set; }

        public long Size { get; set; }

        /// <summary>
        /// MD5 hash of the stream. Could be null. If value is
        /// specified, but actual hash of the data is different
        /// storage should throw ArgumentException
        /// </summary>
        [JsonConverter(typeof(ByteArrayJsonConverter))]
        public byte[] Hash { get; set; }

        /// <summary>
        /// MD5 hash of the compressed stream. Could be null. If value is
        /// specified, but actual hash of the data is different
        /// storage should throw ArgumentException
        /// </summary>
        [JsonConverter(typeof(ByteArrayJsonConverter))]
        public byte[] CompressionHash { get; set; }

        /// <summary>
        /// Indicates if needs to be decompressed after restore
        /// before return for consuming.
        /// Default is false.
        /// </summary>
        public bool DecompressOnRestore { get; set; }

        /// <summary>
        /// True if stream is compressed. Default false
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// The length of the stream. Can be null.
        /// If value is specified, but the actual length
        /// of the Stream is different the storage
        /// should throw ArgumentException
        /// </summary>
        public long? Length { get; set; }

        //[DataMember]
        //public StreamInfo Info { get; set; }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            return this.Key == ((Index) obj).Key;
        }
    }
}
