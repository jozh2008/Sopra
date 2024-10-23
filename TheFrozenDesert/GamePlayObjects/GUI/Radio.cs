using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class Radio
    {
        private readonly SpriteFont mSpriteFont;

        private readonly Vector2 mPosition = new Vector2(100,484);
        private readonly Rectangle mSizeSign = new Rectangle(0,0,1016,200);

        private readonly Vector2 mTitlePosition = new Vector2(100 + 60,484 + 20);

        private readonly Vector2 mCharacterSize = new Vector2(10, 10); 

        private readonly Color mFontColor = Color.Black;
        private readonly Texture2D mBackgroundTexture;

        private readonly String mMessage;

        public Radio(Texture2D backgroundTexture, SpriteFont spriteFont, String message)
        {
            mBackgroundTexture = backgroundTexture;
            mSpriteFont = spriteFont;
            mMessage = message;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            spriteBatch.Draw(mBackgroundTexture, mPosition, mSizeSign ,Color.White);
            DrawText(spriteBatch);
        }

        public Radio Update(GameTime gameTime, Game1.Managers managers)
        {
            if (managers.mInputHandler.MouseClickRight)
            {
                return null;
            }

            return this;
        }
        
        private void DrawText(SpriteBatch spriteBatch)
        {
            // Radio:
            spriteBatch.DrawString(mSpriteFont, "Radio der Dampfmaschine:", mTitlePosition, mFontColor);
            
            // mMessage
            var middle = new Vector2(mPosition.X + mSizeSign.Width / (float)2 - mCharacterSize.X, mPosition.Y + mSizeSign.Height / (float)2 - mCharacterSize.Y); 
            var startX = mMessage.Length / 2;
            var textPosition = middle;
            textPosition.X -= startX * mCharacterSize.X;
            spriteBatch.DrawString(mSpriteFont, mMessage, textPosition, mFontColor);
        }
    }
}