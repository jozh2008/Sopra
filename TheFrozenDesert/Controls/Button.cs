using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheFrozenDesert.Controls
{
    public class Button : MenuComponent
    {
        #region Fields

        private MouseState mCurrentMouse;
        private MouseState mPreviousMouse;
        private readonly SpriteFont mFont;
        private bool mIsHovering;
        private readonly Texture2D mTexture;

        #endregion

        #region properties

        public event EventHandler Click;

        private Color PenColour { get; }
        public Vector2 Position { get; set; }

        private Rectangle Rectangle =>
            new Rectangle((int) Position.X, (int) Position.Y, mTexture.Width, mTexture.Height);

        public string Text { get; set; }

        #endregion

        #region methods

        public Button(Texture2D texture, SpriteFont font)
        {
            mTexture = texture;
            mFont = font;
            PenColour = Color.Black;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;
            if (mIsHovering)
            {
                colour = Color.Gray;
            }

            spriteBatch.Draw(mTexture, Rectangle, colour);
            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + Rectangle.Width / 2 - mFont.MeasureString(Text).X / 2;
                var y = Rectangle.Y + Rectangle.Height / 2 - mFont.MeasureString(Text).Y / 2;
                spriteBatch.DrawString(mFont, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            mPreviousMouse = mCurrentMouse;
            mCurrentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(mCurrentMouse.X, mCurrentMouse.Y, 1, 1);
            mIsHovering = false;
            if (mouseRectangle.Intersects(Rectangle))
            {
                mIsHovering = true;
                if (mCurrentMouse.LeftButton == ButtonState.Released &&
                    mPreviousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        #endregion
    }
}