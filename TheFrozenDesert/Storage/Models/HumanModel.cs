namespace TheFrozenDesert.Storage.Models
{
    public class HumanModel : AbstractGameObjectModel
    {
        private bool mIsDead;
        public int Health { get; set; }
        public int Heat { get; set; }
        public int Saturation { get; set; }
        public int Attack { get; set; }
        public string Faction { get; set; }

        public bool IsDead
        {
            set => mIsDead = value;
        }

        public int GridCurrentPositionX { get; set; }
        public int GridCurrentPositionY { get; set; }
        public bool HasArmor { get; set; }
    }
}