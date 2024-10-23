namespace TheFrozenDesert.Storage.Models
{
    public sealed class SnowTileModel : AbstractGameObjectModel
    {
        public int GridCurrentPositionX { get; set; }
        public int GridCurrentPositionY { get; set; }
        public bool ContainsKey { get; set;}
    }
}
