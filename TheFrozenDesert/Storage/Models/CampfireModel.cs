namespace TheFrozenDesert.Storage.Models
{
    public sealed class CampfireModel : AbstractGameObjectModel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public float TimeToLive { get; set; }
    }
}
