using Microsoft.Xna.Framework;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.Equipment
{
    public sealed class Bow
    {
        public enum BowType
        {
            Metall,
            Holz
        }
        public bool IsDead { get; private set; }
        public int NumberOfArrows { get; set; }
        public int NumberOfArrowsShot { get; set; }
        private readonly BowType mBowType;
        public Bow( BowType bowType, bool isDead, int numberOfArrows = 2, int numberOfArrowsShot = 0)
        {
            IsDead = isDead;
            mBowType = bowType;
            NumberOfArrows = numberOfArrows;
            NumberOfArrowsShot = numberOfArrowsShot;
        }
        public void Update(GameTime gameTime, Grid grid, GameState gameState)
        {
            //imNumberOfArrowsShouted updated in archer after shot, if mNumberOfArrows <= mNumberOfArrowsShot then bow destroy
            if (NumberOfArrows <= NumberOfArrowsShot)
            {
                IsDead = true;
            }
        }

        public string ReturnTypeAsString()
        {
            return mBowType.ToString();
        }

        public BowType ReturnBowType()
        {
            return mBowType;
        }
    }
}
