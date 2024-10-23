using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class EmptyObject : AbstractGameObject
    {
        private bool mIsBlocked;
        public EmptyObject(Texture2D texture, int gridPosX, int gridPosY, Grid grid) : base(texture,
            gridPosX,
            gridPosY,
            grid)
        {
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
        }

        public void Block()
        {
            mIsBlocked = true;
        }

        public bool IsBlocked()
        {
            return mIsBlocked;
        }
    }
}