using System;

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
