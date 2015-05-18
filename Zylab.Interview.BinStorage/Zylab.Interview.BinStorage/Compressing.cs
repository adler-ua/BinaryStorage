using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Zylab.Interview.BinStorage
{
    public static class Compressing
    {
        public static Stream Compress(Stream source)
        {
            MemoryStream outStream = new MemoryStream();
            {
                using(GZipStream compress = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    source.CopyTo(compress);
                    compress.Flush();
                }
                return outStream;
            }
        }

        public static Stream Decompress(Stream source)
        {
            MemoryStream outStream = new MemoryStream();
            using (GZipStream decompress = new GZipStream(source, CompressionMode.Decompress, true))
            {
                decompress.CopyTo(outStream);
                decompress.Flush();
            }
        	
            return outStream;
        }

    }
}
