using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    internal class HumanGui : AbstractGameObjectGui
    {
        private readonly Human mHuman;
        private readonly List<GuiItemSlot> mSlots = new List<GuiItemSlot>();

        public HumanGui(GameState gameState, Human human) : base(human,
            gameState.mGameObjectGuiTexture,
            new Point(150,
                185),
            human.ReturnHumanType(human),
            gameState.mSpriteFont)
        {
            mHuman = human;
            mSlots.Add(new GuiItemSlot(gameState.mInputHandler,
                new Point(100, 30),
                new Point(32, 32),
                gameState,
                "Tool",
                AbstractEquipment.EquipmentSlotType.Tool,
                human));
            mSlots.Add(new GuiItemSlot(gameState.mInputHandler,
                new Point(100, 60),
                new Point(32, 32),
                gameState,
                "Armor",
                AbstractEquipment.EquipmentSlotType.Armor,
                human));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            base.Draw(gameTime, spriteBatch,gameState);
            foreach (var slot in mSlots)
            {

                slot.Draw(gameTime, spriteBatch);
            }

            var rectangle = new Rectangle(mAbstractGameObject.GetPos().X + 32,
                mAbstractGameObject.GetPos().Y + 30,
                mSize.X,
                mSize.Y);
            var text = "Tool: ";
            var x = rectangle.X + 15;
            var y = rectangle.Y + 30 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
            text = "Armor: ";
            x = rectangle.X + 15;
            y = rectangle.Y + 60 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
            var info = mHuman.GetHumanInfo();
            text = "Health: " + info.mHealth + " HP";
            x = rectangle.X + 15;
            y = rectangle.Y + 90 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
            text = "Saturation: " + info.mSaturation + "%";
            x = rectangle.X + 15;
            y = rectangle.Y + 110 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
            text = "Heat: " + info.mHeat + "C";
            x = rectangle.X + 15;
            y = rectangle.Y + 130 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
            text = "Faction: " + info.mFaction;
            x = rectangle.X + 15;
            y = rectangle.Y + 150 - mFont.MeasureString(text).Y;
            spriteBatch.DrawString(mFont, text, new Vector2(x, y), Color.White);
        }
    }
}