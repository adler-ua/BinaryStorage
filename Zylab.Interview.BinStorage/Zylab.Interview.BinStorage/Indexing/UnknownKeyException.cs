using System;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class UnknownKeyException: Exception
    {
        public UnknownKeyException(string key) : base(String.Format("Could not find index record with key '{0}'", key))
        {
            
        }
    }
}
