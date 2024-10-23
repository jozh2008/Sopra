using System;
using System.IO;


namespace TheFrozenDesert.Storage
{
    public class DirectoryManager
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public const string SavingsFileName = "savings.json";
        public static readonly string SavingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SavingsFileName);

        public static string CombineBaseDirectoryWithFile(string file)
        {
            return Path.Combine(BaseDirectory, file);
        }

    }
}
