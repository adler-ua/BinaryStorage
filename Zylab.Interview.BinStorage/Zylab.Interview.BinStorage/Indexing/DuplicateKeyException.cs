using System;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException(string key)
            : base(string.Format("Attempt to add index with key '{0}' which already exists.", key))
        {
            
        }
    }
}
