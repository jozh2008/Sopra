using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class GuiItemSlot
    {
        private readonly AbstractEquipment.EquipmentSlotType mEquipmentSlotType;
        private readonly SpriteFont mFont;
        private readonly Human mHuman;
        private readonly InputHandler mInputHandler;
        private readonly Point mRelativePosition;
        private readonly Point mSize;
        private readonly string mText;
        private readonly Texture2D mTexture;

        public GuiItemSlot(InputHandler inputHandler,
            Point relativePosition,
            Point size,
            GameState gameState,
            string text,
            AbstractEquipment.EquipmentSlotType equipmentSlotType,
            Human human)
        {
            mInputHandler = inputHandler;
            mRelativePosition = relativePosition;
            mSize = size;
            mTexture = gameState.mGuiItemSlotTexture;
            mFont = gameState.mSpriteFont;
            mText = text;
            mEquipmentSlotType = equipmentSlotType;
            mHuman = human;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;
            var rectangle = new Rectangle(mHuman.GetPos().X + 32 + mRelativePosition.X,
                mHuman.GetPos().Y + mRelativePosition.Y,
                mSize.X,
                mSize.Y);
            var mouseRectangle = new Rectangle((int) mInputHandler.MousePosition.X,
                (int) mInputHandler.MousePosition.Y,
                1,
                1);
            if (mouseRectangle.Intersects(rectangle))
            {
                color = Color.Gray;
            }

            spriteBatch.Draw(mTexture, rectangle, color);
            if (!string.IsNullOrEmpty(mText) && mouseRectangle.Intersects(rectangle))
            {
                var x = (int) mInputHandler.MousePosition.X;
                var y = (int) mInputHandler.MousePosition.Y + 20;
                var drawBackground =
                    new Rectangle(x, y, (int) mFont.MeasureString(mText).X, (int) mFont.MeasureString(mText).Y);
                spriteBatch.FillRectangle(drawBackground, Color.Black);
                spriteBatch.DrawString(mFont, mText, new Vector2(x, y), Color.White);
            }

            if (!(mHuman.GetEquipment(mEquipmentSlotType) is null))
            {
                mHuman.GetEquipment(mEquipmentSlotType)
                    .Draw(gameTime, spriteBatch, new Point(rectangle.X, rectangle.Y));
            }
        }
    }
}