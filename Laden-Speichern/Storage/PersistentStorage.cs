using Newtonsoft.Json;
using System.IO;

namespace TheFrozenDesert.Storage
{
    
    public class PersistentStorage
    {

        public void WriteGameState<T>(T data, string file)
        {
            File.WriteAllText(DirectoryManager.CombineBaseDirectoryWithFile(file), JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public T ReadGameState<T>(string file)
        {
            using StreamReader r = new StreamReader(DirectoryManager.CombineBaseDirectoryWithFile(file));
            return JsonConvert.DeserializeObject<T>(r.ReadToEnd());

        }
    }
}
