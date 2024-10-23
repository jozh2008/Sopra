using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Key : AbstractGameObject
    {
        private bool mGameStateUpdatet;
        public Key(Texture2D texture, int gridPosX, int gridPosY, Grid grid) : base(texture, gridPosX, gridPosY, grid)
        {
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            if (!(mGrid.GetAbstractGameObjectAt(new Point(mGridPos.X, mGridPos.Y)) is Key) && !mGameStateUpdatet)
            {
                gameState.mKeyCounter += 1;
                mGameStateUpdatet = true;
                if (gameState.mKeyCounter == 3)
                {
                    gameState.mGame.AchievementsModel.mKeyKing = true;
                }
                //gameState.mGrid.RemoveKey(this);
            }
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new KeyModel()
            {
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y
            };
        }
    }
}