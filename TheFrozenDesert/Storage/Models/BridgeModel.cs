namespace TheFrozenDesert.Storage.Models
{
    public sealed class BridgeModel : AbstractGameObjectModel
    {
        public float mRepairState;
        public int GridCurrentPositionX { get; set; }
        public int GridCurrentPositionY { get; set; }
    }
}
