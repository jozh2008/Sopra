namespace TheFrozenDesert.Storage.Models
{
    public sealed class WolfModel : AbstractGameObjectModel
    {
        public int Health { get; set; }
        public int HealthRegenerationrate { get; set; }
        public int AttackRadius { get; set; }
        public int Damage { get; set; }
        public string Uuid { get; set; }
        public int GridCurrentPositionX { get; set; }
        public int GridCurrentPositionY { get; set; }
    }
}
