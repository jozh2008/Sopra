using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class SignGui : AbstractGameObjectGui
    {
        private readonly Sign mSign;

        private readonly SpriteFont mSpriteFont;

        private readonly Vector2 mPosition = new Vector2(100,484);
        private readonly Rectangle mSizeSign = new Rectangle(0,0,1016,200);

        private readonly Vector2 mCharacterSize = new Vector2(10, 10); 

        private readonly Color mFontColor = Color.Black;
        private readonly Texture2D mBackgroundTexture;
        
        public SignGui(Sign sign, Texture2D backgroundTexture, SpriteFont spriteFont): base (sign, backgroundTexture, new Point(300,100), "sign", spriteFont)
        {
            mSign = sign;
            mBackgroundTexture = backgroundTexture;
            mSpriteFont = spriteFont;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            spriteBatch.Draw(mBackgroundTexture, mPosition, mSizeSign ,Color.White);
            DrawText(spriteBatch);
        }
        
        private void DrawText(SpriteBatch spriteBatch)
        {
            var middle = new Vector2(mPosition.X + mSizeSign.Width / (float)2 - mCharacterSize.X, mPosition.Y + mSizeSign.Height / (float)2 - mCharacterSize.Y); 
            var startX = mSign.mMessage.Length / 2;
            var textPosition = middle;
            textPosition.X -= startX * mCharacterSize.X;
            spriteBatch.DrawString(mSpriteFont, mSign.mMessage, textPosition, mFontColor);
        }
    }
}