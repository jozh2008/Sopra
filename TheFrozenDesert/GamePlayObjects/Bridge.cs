using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Bridge: AbstractGameObject
    {
        private float mRepairState;
        private readonly float mRepairStep;
        private readonly Texture2D mRepairedTexture;

        public Bridge(
            Texture2D texture, 
            Texture2D repairedTexture,
            int posX, 
            int posY,
            Grid grid,
            float repairState = 0, 
            float repairStep = 0.2f)
            : base(texture, posX, posY, grid)
        {
            mRepairedTexture = repairedTexture;
            mRepairState = repairState;
            mRepairStep = repairStep;
        }

        public Bridge(
            Texture2D texture,
            Texture2D repairedTexture,
            BridgeModel model,
            Grid grid,
            float repairStep = 0.2f)
            : base(texture, model.GridCurrentPositionX, model.GridCurrentPositionY, grid)
        {
            mRepairedTexture = repairedTexture;
            mRepairState = model.mRepairState;
            mRepairStep = repairStep;
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
        }

        public float Repair(GameTime gameTime)
        {
            if (mRepairState < 1)
            {
                mRepairState += mRepairStep * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (mRepairState >= 1)
            {
                SetRepaired();
            }

            return mRepairState;
        }

        public void SetRepaired()
        {
                mTexture = mRepairedTexture;
                mGrid.AddNonInteractableObject(this);
                mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
        }
        public bool IsRepaired()
        {
            return (mRepairState >= 1);
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new BridgeModel()
            {
                mRepairState = mRepairState,
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y
            };
        }
    }
}
