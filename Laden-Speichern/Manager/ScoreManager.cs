using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using TheFrozenDesert.Storage;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.Manager
{
   public class ScoreManager
   {
        private static string _fileName = DirectoryManager.SavingsFile;
        private static string PATH = DirectoryManager.SavingsFileName;


       public List<Score> HighScores { get; private set; }
       public List<Score> Scores { get; private set; }

       public ScoreManager()
           : this(new List<Score>())
       {
          
       }
       public ScoreManager(List<Score> scores)
       {
           Scores = scores;
           UpdateHighscore();
       }

       public void Add(Score score)
       {
           Scores.Add(score);
           Scores = Scores.OrderByDescending(c => c.Highscore).ToList();
           UpdateHighscore();
       }

       public static ScoreManager Load()
       {
           if (!File.Exists(_fileName)) 
               return new ScoreManager();

            
                       using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
                       {
                           List<Score> scores = JsonConvert.DeserializeObject<List<Score>>(reader.ReadToEnd());
                           return new ScoreManager(scores);

                       }

            
           
        }

       public void UpdateHighscore()
       {
           HighScores = Scores.Take(5).ToList();
       }

       public static void Save(ScoreManager scoreManager)
       {

           using (var writer = new StreamWriter(new FileStream(DirectoryManager.CombineBaseDirectoryWithFile(PATH), FileMode.Create)))
           {
               string serializedText = JsonConvert.SerializeObject(scoreManager.Scores, Formatting.Indented); 
               writer.WriteLine(serializedText);
           }


               //string serializedText = JsonConvert.SerializeObject(scoreManager.Scores, Formatting.Indented);
               //File.WriteAllText((_fileName), serializedText);
          
       }
   }
}
