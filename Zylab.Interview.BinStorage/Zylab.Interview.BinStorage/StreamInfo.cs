using Newtonsoft.Json;
using Zylab.Interview.BinStorage.JsonUtils;

namespace Zylab.Interview.BinStorage {
    
    public class StreamInfo {
        public static readonly StreamInfo Empty = new StreamInfo();

        public StreamInfo()
        {
            CompressionHash = new byte[0];
        }

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

        public object Clone()
        {
            var clone = new StreamInfo()
            {
                IsCompressed = IsCompressed,
                Length = Length,
                DecompressOnRestore = DecompressOnRestore
            };
            if (Hash != null)
            {
                clone.Hash = new byte[Hash.Length];
                Hash.CopyTo(clone.Hash, 0);
            }

            if (CompressionHash != null)
            {
                clone.CompressionHash = new byte[CompressionHash.Length];
                CompressionHash.CopyTo(clone.CompressionHash, 0);
            }
            return clone;
        }
    }
}
