namespace TheFrozenDesert.Storage.Models
{
    public sealed class GathererModel : HumanModel
    {
        public double mMiningSpeed;
        public int mEarningIncreaseRock; // Value is added to Earning increases if Gatherer mines often
        public int mEarningIncreaseTree; // Value is added to Earning increases if Gatherer mines often
        public int mNumberOfTimeCutTree;
        public int mNumberOfTimeMineRock;
        public double mTimeSinceLastMining;
        public bool mHasAxe;
        public string mAxeMaterial;
        public int? mAxeUses;
    }
}