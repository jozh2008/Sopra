namespace TheFrozenDesert.Storage.Models
{
    public sealed class WolfPackModel : AbstractGameObjectModel
    {
        public string[] Uuids { get; set;}
        public int AreaX { get; set; }
        public int AreaY { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
    }
}
