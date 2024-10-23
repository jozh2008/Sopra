using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class GamePlayObjectInfoBar
    {
        private const int SizeX = 32;
        private const int SizeY = 5;
        private int mScale = 1;
        private Rectangle mBackgroundRectangle;
        public Color mForegroundColor;
        private readonly AbstractGameObject mGameObject;
        private readonly int mRelativeHeight;
        private Texture2D mBlankTexture;
        private Rectangle mForgroundRectangle;

        private int mPercent = 100;
        private Vector2 mPositionBackgroundRectangle;
        private Vector2 mPositionForgroundRectangle;

        public GamePlayObjectInfoBar(AbstractGameObject gameObject, int relativeHeight, Color color)
        {
            mGameObject = gameObject;
            mRelativeHeight = relativeHeight;
            mBackgroundRectangle = new Rectangle(0, 0, SizeX, SizeY);
            mForgroundRectangle = new Rectangle(0, 0, SizeX - 2, SizeY - 2);
            mForegroundColor = color;
        }

        public void SetScale(int scale)
        {
            mForgroundRectangle.Width *= scale;
            mForgroundRectangle.Height *= scale;
            mBackgroundRectangle.Width *= scale;
            mBackgroundRectangle.Height *= scale;
            mScale = scale;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            GetBlankTexture(spriteBatch);

            UpdatePositions();
            UpdateRectangles();
            spriteBatch.Draw(mBlankTexture,
                mPositionBackgroundRectangle - camera.PositionPixels,
                mBackgroundRectangle,
                Color.Black);
            spriteBatch.Draw(mBlankTexture,
                mPositionForgroundRectangle - camera.PositionPixels,
                mForgroundRectangle,
                mForegroundColor);
        }

        private void UpdatePositions()
        {
            mPositionBackgroundRectangle = new Vector2(mGameObject.GetAbsolutePos().X, mGameObject.GetAbsolutePos().Y);

            // change position so bar is on the bottom of the Human
            mPositionBackgroundRectangle.Y += mRelativeHeight;
            // move Forground Rectangel 1,1 pixels 
            mPositionForgroundRectangle = mPositionBackgroundRectangle;
            mPositionForgroundRectangle.X += 1 * mScale;
            mPositionForgroundRectangle.Y += 1 * mScale;
        }

        private void UpdateRectangles()
        {
            // Update Forground Rectangle so it fits to mPercent
            var widthFull = mBackgroundRectangle.Width - 2;

            var widthNew = (int) (widthFull / 100.0f * mPercent);
            mForgroundRectangle.Width = widthNew;
        }

        public void Decrease(int percent)
        {
            var percentNew = mPercent - percent;
            if (percentNew > 0)
            {
                mPercent = percentNew;
            }
            else
            {
                mPercent = 0;
            }
        }

        public void SetPercent(int percent)
        {
            if (percent <= 100 && percent >= 0)
            {
                mPercent = percent;
            }
        }

        private void GetBlankTexture(SpriteBatch spriteBatch)
        {
            if (mBlankTexture == null)
            {
                mBlankTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                mBlankTexture.SetData(new[] {Color.White});
            }
        }
    }
}