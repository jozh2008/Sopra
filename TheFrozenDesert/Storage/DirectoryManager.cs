using System;
using System.IO;

namespace TheFrozenDesert.Storage
{
    internal static class DirectoryManager
    {
        private static readonly string sBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //public static readonly string ResourceObjectsFile = Path.Combine(AppDomain.CurrentDomain.sBaseDirectory, SavingsFileName);

        public static string CombineBaseDirectoryWithFile(string file)
        {
            return Path.Combine(sBaseDirectory + "/saves", file);
        }

        public static void CreateGamestateDirectoryIfEmpty(string directoryId)
        {
            var absPath = Path.Combine(sBaseDirectory + "/saves", "GameStates/" + directoryId + "/");
            if (!Directory.Exists(absPath))
            {
                Directory.CreateDirectory(absPath);
                File.Create(absPath + "ResourceObjects.json", int.MaxValue).Dispose();
            }
        }
        public static void CreateAchievementDirectoryIfEmpty()
        {
            var absPath = Path.Combine(sBaseDirectory + "/saves", "Achievements");
            if (!Directory.Exists(absPath))
            {
                Directory.CreateDirectory(absPath);
                File.Create(absPath + "Achievements.json", int.MaxValue).Dispose();
            }
        }

        public static void CreateStatisticsDirectoryIfEmpty()
        {
            var absPath = Path.Combine(sBaseDirectory + "/saves", "Statistics");
            if (!Directory.Exists(absPath))
            {
                Directory.CreateDirectory(absPath);
                File.Create(absPath + "Statistics.json", int.MaxValue).Dispose();
            }
        }
    }
}