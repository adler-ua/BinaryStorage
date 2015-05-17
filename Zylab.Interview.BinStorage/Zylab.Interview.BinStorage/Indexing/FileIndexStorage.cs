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
        private readonly string _path;
        private const string IndexFileName = "index.json";

        public FileIndexStorage(string directory)
        {
            _path = Path.Combine(directory, IndexFileName);
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

        public void Save(Index item)
        {
            JsonConvert.SerializeObject(item, Formatting.Indented);
            using (FileStream fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (fileStream.Length == 0)
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        jsonWriter.Formatting = Formatting.Indented;
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jsonWriter, new Dictionary<string, Index>() {{item.Key, item}});
                    }
                }
                else
                {
                    fileStream.Seek(-1, SeekOrigin.End);
                    string itemSerialized = JsonConvert.SerializeObject(item, Formatting.Indented);
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        sw.Write(",\n{0}: {1} }}", JsonUtils.EncodeJsString(item.Key), itemSerialized);
                    }
                }
            }
        }

        public Dictionary<string, Index> Restore()
        {
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
