using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class IncorrectHashException : Exception
    {
        public IncorrectHashException(string key)
            : base(string.Format("Hashes do not match for file - '{0}'", key))
        {
            
        }
    }
}
