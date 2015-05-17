using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class UnknownKeyException: Exception
    {
        public UnknownKeyException(string key) : base(String.Format("Could not find index record with key '{0}'", key))
        {
            
        }
    }
}
