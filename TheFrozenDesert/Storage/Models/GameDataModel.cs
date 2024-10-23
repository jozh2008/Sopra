using System.Collections.Generic;

namespace TheFrozenDesert.Storage.Models
{
    public class GameDataModel
    {
        public int FleischCount { get; set; }
        public int MetallCount { get; set; }
        public int WoodCount { get; set; }
        public int KillCount { get; set; }
        public int KeyCount { get; set; }
        public double Timer { get; set; }
        public int EnemyFleischCount { get; set; }
        public int EnemyMetallCount { get; set; }
        public int EnemyWoodCount { get; set; }

        public int VictoryState { get; set; }
        public Dictionary<string, int> Tools { get; set; }
    }
}