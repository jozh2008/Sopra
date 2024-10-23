using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class ReactorPart : AbstractGameObject
    {
        public ReactorPart(Texture2D texture, int gridPosX, int gridPosY, Grid grid, Point textureRegion) : base(texture, gridPosX, gridPosY, grid)
        {
            mTextureRegion = new Rectangle(textureRegion.X, textureRegion.Y, 32, 32);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {}
    }
}
