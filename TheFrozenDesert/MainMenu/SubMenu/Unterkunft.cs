using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.MainMenu.SubMenu
{
    public class Unterkunft: Sledge 
    {
        private const int Capacity = 5;
        public Unterkunft(Texture2D texture, int gridPosX, int gridPosY, float movementSpeed, Sledge nextSledge, Grid grid ):
            base(texture, gridPosX, gridPosY, movementSpeed, nextSledge, grid)
        {
            mTextureRegion = new Rectangle(32, 0, 32, 32);
        }

        public Unterkunft(HousingModel model,
            GameState gameState,
            Texture2D texture2D,
            float movementSoeed,
            Grid grid) :
            base(gameState, texture2D, model, movementSoeed, grid)
        {
            mTextureRegion = new Rectangle(32, 0, 32, 32);
            gameState.mCapacityHumanInSledge += Capacity;
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var sledgeModel = new HousingModel()
            {
                X = mGridPos.X + gameState.GetGridOffset().X,
                Y = mGridPos.Y + gameState.GetGridOffset().Y,
                PreviousSledgeUuid = mPreviousSledge is null ? "none" : mPreviousSledge.Uuid.ToString(),
                Uuid = Uuid.ToString()
            };
            return sledgeModel;
        }
        public void AddCapacity(GameState gameState)

        {
            gameState.mCapacityHumanInSledge += Capacity;
        }
    }
}
