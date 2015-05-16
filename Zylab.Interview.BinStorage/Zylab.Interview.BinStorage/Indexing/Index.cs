using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class Index
    {
        public Index(string key, int offset, int size, StreamInfo info)
        {
            if (string.IsNullOrEmpty(key))  throw new ArgumentException("Key should not be empty.","key");
            if (info == null)               throw new ArgumentNullException("info");
            Key = key;
            Offset = offset;
            Size = size;
        }

        public string Key { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }
    }
}
