using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Zylab.Interview.BinStorage.JsonUtils;

namespace Zylab.Interview.BinStorage {
    
//    [DataContract]
    public class StreamInfo : ICloneable {
        public static readonly StreamInfo Empty = new StreamInfo();

        /// <summary>
        /// MD5 hash of the stream. Could be null. If value is
        /// specified, but actual hash of the data is different
        /// storage should throw ArgumentException
        /// </summary>
        [JsonConverter(typeof(ByteArrayJsonConverter))]
        public byte[] Hash { get; set; }

        /// <summary>
        /// True if stream is compressed. Default false
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Indicates if needs to be decompressed after restore
        /// before return for consuming.
        /// Default is false.
        /// </summary>
        public bool DecompressOnRestore { get; set; }

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
                DecompressOnRestore = DecompressOnRestore,
                IsCompressed = IsCompressed,
                Length = Length
            };
            if (Hash != null)
            {
                clone.Hash = (byte[]) Hash.Clone();
            }
            return clone;
        }
    }

}
