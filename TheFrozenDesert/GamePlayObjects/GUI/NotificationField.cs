using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public class NotificationField
    {
        private readonly SpriteFont mFont;
        public NotificationField(SpriteFont spriteFont)
        {
            mFont = spriteFont;
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            
            var backgroundColor = Color.Black;
            var sizeY = 10;
            
            sizeY += (int)mFont.MeasureString(gameState.mHasEnoughWoodForMovingSledge).Y + 5;
            sizeY += (int)mFont.MeasureString(gameState.mHasEnoughResourceForBuildingItems).Y + 5;
            sizeY += (int)mFont.MeasureString(gameState.mHasEnoughResourceForBuildingSledgeStation).Y + 5;
            sizeY += (int)mFont.MeasureString(gameState.mHasEnoughResourceForCookingFood).Y + 5;
            sizeY += (int)mFont.MeasureString(gameState.mNotificationForRecruting).Y + 5;
            var rectangle = new Rectangle(896, 32, 288, sizeY);
            spriteBatch.FillRectangle(rectangle, backgroundColor);
            var color = Color.White;
            if (gameState.mKaminIsActivated)
            {
                var text = "Kamin ist aktiv. Verbrauch: 1 Holz/5s";
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
               
            }
            else if(!gameState.mHasKamin && !gameState.mKaminIsActivated)
            {
                var text = "Kamin ist nicht aktiv, da man kein Kamin hat";
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            else if(gameState.mHasKamin && !gameState.mKaminIsActivated && gameState.mResources.Get(ResourceType.Wood)==0)
            {
                var text = "Kamin ist nicht aktiv, da man kein Holz hat";
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            else
            {
                var text = "Kamin ist nicht aktiv, da es ausgeschaltet ist";
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            if (gameState.mHasDampfmaschine && (gameState.mHasEnoughWoodForMovingSledge.Length>0))
            {
                var text = gameState.mHasEnoughWoodForMovingSledge;
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y + (int)mFont.MeasureString(text).Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            if (gameState.mHasEnoughResourceForBuildingItems.Length > 0)
            {
                var text = gameState.mHasEnoughResourceForBuildingItems;
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y + (int)mFont.MeasureString(text).Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }

            if (gameState.mHasEnoughResourceForBuildingSledgeStation.Length > 0)
            {
                var text = gameState.mHasEnoughResourceForBuildingSledgeStation;
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y + (int)mFont.MeasureString(text).Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            if (gameState.mHasEnoughResourceForCookingFood.Length > 0)
            {
                var text = gameState.mHasEnoughResourceForCookingFood;
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y + (int)mFont.MeasureString(text).Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }
            if (gameState.mNotificationForRecruting.Length > 0)
            {
                var text = gameState.mNotificationForRecruting;
                var x = rectangle.X + rectangle.Width / 2 - gameState.mSpriteFontNotification.MeasureString(text).X / 2;
                var y = rectangle.Y + (int)mFont.MeasureString(text).Y;
                spriteBatch.DrawString(gameState.mSpriteFontNotification, text, new Vector2(x, y), color);
            }


        }
       
    }
}
