﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public interface IPersistentStreamStorage : IDisposable
    {
        void SaveFile(Stream data, out long offset, out long size);

        Stream RestoreFile(long offset, long size);
    }
}
