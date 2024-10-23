using Microsoft.Xna.Framework;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.Equipment
{
    public sealed class Sword
    {
        public enum SwordType
        {
            Metall,
            Holz
        }

        public bool IsDead { get; private set; }
        public int NumberOfUsesSword { get; set; } //initally 0
        private readonly int mTotalNumberOfUsesSwordHas;// gives how many times the axe can be used, more it gets destroyes
        private readonly SwordType mSwordType;
        public Sword(SwordType swordType, bool isDead, int totalNumberOfUsesSwordHas = 2, int numberOfUsesSword = 0)
        {
            IsDead = isDead;
            mSwordType = swordType;
            NumberOfUsesSword = numberOfUsesSword;
            mTotalNumberOfUsesSwordHas = totalNumberOfUsesSwordHas;
        }
        public void Update(GameTime gameTime, Grid grid, GameState gameState)
        {
            

            //mNumberOfUsesAxe updated in gatherer after punch, if mTotalNumberOfUsesAxeHas <= mNumberOfUsesAxe then axe destroy
            if (mTotalNumberOfUsesSwordHas <= NumberOfUsesSword)
            {
                IsDead = true;
            }
        }

        public string ReturnTypeAsString()
        {
            return mSwordType.ToString();
        }
    }
}
