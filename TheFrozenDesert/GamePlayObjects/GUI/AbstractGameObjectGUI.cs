using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class AbstractGameObjectGui
    {
        protected readonly AbstractGameObject mAbstractGameObject;
        protected readonly List<AbstractGameObjectGuiButton> mButtons;
        protected readonly SpriteFont mFont;
        protected readonly Point mSize;
        private readonly string mText;
        private readonly Texture2D mTexture;

        public int mMaxY = 720;
        private Point mPos;

        public AbstractGameObjectGui(AbstractGameObject gameObject,
            Texture2D texture,
            Point size,
            string text,
            SpriteFont font)
        {
            mAbstractGameObject = gameObject;
            mTexture = texture;
            mSize = size;
            mText = text;
            mFont = font;
            mButtons = new List<AbstractGameObjectGuiButton>();

            mPos = new Point(mAbstractGameObject.GetPos().X, Math.Min(mAbstractGameObject.GetPos().Y, mMaxY));
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch,GameState gameState)
        {
            UpdatePos();
            
            var colour = Color.White;
            var rectangle = new Rectangle(mPos.X + 32,
                mPos.Y,
                mSize.X,
                mSize.Y);
            spriteBatch.Draw(mTexture, rectangle, colour);
            if (!string.IsNullOrEmpty(mText))
            {
                var x = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(mText).X / 2;
                var y = rectangle.Y + 5;
                spriteBatch.DrawString(mFont, mText, new Vector2(x, y), Color.White);
            }

            foreach (var abstractGameObjectGuiButton in mButtons)
            {
                abstractGameObjectGuiButton.Draw(spriteBatch);
            }
            foreach (var abstractGameObjectGuiButton in gameState.mButtonsAbstractGameObjectGui)
            {
                abstractGameObjectGuiButton.Draw(spriteBatch);
            }
        }

        private void UpdatePos()
        {
            mPos = new Point(mAbstractGameObject.GetPos().X, Math.Min(mAbstractGameObject.GetPos().Y, mMaxY));
        }

        public bool HitboxCheckLeftClick(GameState gameState, InputHandler inputHandler)
        {
            var rectangle = new Rectangle(mPos.X + 32,
                mPos.Y,
                mSize.X,
                mSize.Y);
            //Debug.WriteLine(gameState.mButtons.Count);
            foreach (var button in mButtons)
            {
                button.HitboxCheck(gameState, inputHandler);
            }
            foreach (var button in gameState.mButtonsAbstractGameObjectGui)
            {
                button.HitboxCheck(gameState, inputHandler);
            }


            var selected = inputHandler.Inputs.IsSelected(rectangle);
            if (selected)
            {
                InputHandler.InputList.BlockFurtherInput();
            }
            return selected;
        }

        public bool HitboxCheckRightClick(GameState gameState, InputHandler inputHandler)
        {
            var rectangle = new Rectangle(mPos.X + 32,
                mPos.Y,
                mSize.X,
                mSize.Y);
            
            return inputHandler.Inputs.IsOpeningGui(rectangle);
        }
    }
}