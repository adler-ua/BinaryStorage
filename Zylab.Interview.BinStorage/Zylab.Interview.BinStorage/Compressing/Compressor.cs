using System.IO;
using System.Security.Cryptography;

namespace Zylab.Interview.BinStorage.Compressing
{
    public class Compressor : ICompressor
    {
        public Stream CompressIfRequired(Stream data, StreamInfo parameters, long compressionThreshold)
        {
            Stream compressedStream = null;
            if (!parameters.IsCompressed)
            {
                if (compressionThreshold > 0 && parameters.Length > compressionThreshold)
                {
                    compressedStream = GZipCompressor.Compress(data);
                    compressedStream.Seek(0, SeekOrigin.Begin);
                    parameters.DecompressOnRestore = true;
                    parameters.Length = compressedStream.Length;
                    using (MD5 md5 = MD5.Create())
                    {
                        parameters.CompressionHash = md5.ComputeHash(compressedStream);
                        compressedStream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            return compressedStream;
        }
    }
}
