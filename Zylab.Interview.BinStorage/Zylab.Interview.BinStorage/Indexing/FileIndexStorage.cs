using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Zylab.Interview.BinStorage.Indexing
{
    public class FileIndexStorage : IPersistentIndexStorage
    {
        private readonly string _directory;
        private readonly string _path;
        private const string IndexFileName = "index.json";

        public FileIndexStorage(string directory)
        {
            _directory = directory;
            _path = Path.Combine(_directory, IndexFileName);
        }

        public void Save(Dictionary<string, Index> items)
        {
            using (FileStream fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (JsonWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, items);
            }
        }

        public Dictionary<string, Index> Restore()
        {
            string path = Path.Combine(_directory, IndexFileName);
            if (!File.Exists(_path))
            {
                return new Dictionary<string, Index>();
            }
            using (StreamReader sr = File.OpenText(_path))
            {
                JsonSerializer serializer = new JsonSerializer();
                var indices = (Dictionary<string, Index>)serializer.Deserialize(sr, typeof (Dictionary<string, Index>));
                return indices;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
