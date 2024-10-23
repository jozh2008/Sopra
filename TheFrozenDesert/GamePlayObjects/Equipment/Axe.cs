using Microsoft.Xna.Framework;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.Equipment
{
    public sealed class Axe
    {
        public enum AxeType
        {
            Metall,
            Holz
        }

        public bool IsDead { get; private set; }
        public int NumberOfUsesAxe { get; set; } //initally 0
        private readonly int mTotalNumberOfUsesAxeHas;// gives how many times the axe can be used, more it gets destroyes
        private readonly AxeType mAxeType;
        public Axe(AxeType axeType, bool isDead, int totalNumberOfUsesAxeHas = 5, int numberOfUsesAxe = 0)
        {
            IsDead = isDead;
            mAxeType = axeType;
            NumberOfUsesAxe = numberOfUsesAxe;
            mTotalNumberOfUsesAxeHas = totalNumberOfUsesAxeHas;
        }
        public void Update(GameTime gameTime, Grid grid, GameState gameState)
        {
            //Debug.WriteLine(mNumberOfUsesAxe);
           

            //mNumberOfUsesAxe updated in gatherer after punch, if mTotalNumberOfUsesAxeHas <= mNumberOfUsesAxe then axe destroy
            if (mTotalNumberOfUsesAxeHas <= NumberOfUsesAxe)
            {
                IsDead = true;
            }
        }

        public string ReturnTypeAsString()
        {
            return mAxeType.ToString();
        }
    }
}

