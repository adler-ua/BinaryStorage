using System.IO;

namespace Zylab.Interview.BinStorage.FileStorage
{
    public class FileStreamStorage : IPersistentStreamStorage
    {
        private readonly string _path;
        private const string StreamStorageFileName = "storage.bin";
        private readonly object _locker = new object();

        public FileStreamStorage(string directory)
        {
            _path = Path.Combine(directory, StreamStorageFileName);
        }

        public void SaveFile(Stream data, out long offset, out long size)
        {
            lock (_locker)
            {
                using (FileStream stream = new FileStream(_path, FileMode.Append))
                {
                    offset = stream.Length;
                    size = data.Length;
                    data.CopyTo(stream);
                }
            }
        }

        public Stream OpenReadStorageStream()
        {
            return File.OpenRead(_path);
        }

        public void Dispose()
        {
            
        }
    }
}
