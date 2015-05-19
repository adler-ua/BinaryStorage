using System.IO;

namespace Zylab.Interview.BinStorage.Compressing
{
    public interface ICompressor
    {
        Stream CompressIfRequired(Stream data, StreamInfo parameters, long compressionThreshold);
    }
}
