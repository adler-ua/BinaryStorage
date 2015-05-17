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
            Info = StreamInfo.Empty;
        }

        public Index(string key, long offset, long size, StreamInfo info)
        {
            if (string.IsNullOrEmpty(key))  throw new ArgumentException("Key should not be empty.","key");
            if (info == null)               throw new ArgumentNullException("info");
            Key = key;
            Offset = offset;
            Size = size;
            Info = info;
        }

        public string Key { get; set; }

        public long Offset { get; set; }

        public long Size { get; set; }

        public StreamInfo Info { get; set; }
    }
}
