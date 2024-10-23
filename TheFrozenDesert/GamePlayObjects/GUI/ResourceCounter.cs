using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public sealed class ResourceCounter
    {
        private readonly SpriteFont mFont;

        public ResourceCounter(SpriteFont spriteFont)
        {
            mFont = spriteFont;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameState gameState)
        {
            var colorBackground = Color.Black;
            if (HitboxCheck(gameState.mInputHandler.MousePosition))
            {
                colorBackground *= 0.25f;
            }

            var sizeY = 10; 
            foreach (var pair in gameState.mResources.ResourceDictionary)
            {
                var text = gameState.mResources.GetNameAsString(pair.Key) + ": " + pair.Value;
                sizeY += (int) mFont.MeasureString(text).Y + 5;
            }
            sizeY += (int)mFont.MeasureString("Kills: " + gameState.mKillCounter).Y + 5;
            sizeY += (int)mFont.MeasureString("Kapazität: " + gameState.mCapacityHumanInSledge).Y + 5;
            sizeY += (int)mFont.MeasureString("Schlüssel: " + gameState.mKeyCounter).Y + 5;
            sizeY += (int)mFont.MeasureString("Fps: " + gameState.CurrentFps).Y + 5;
            sizeY += (int)mFont.MeasureString("Einheiten: " + gameState.mPlayerAliveHumans).Y + 5;
            var rectangle = new Rectangle(32, 32, 128, sizeY);
            spriteBatch.FillRectangle(rectangle, colorBackground);
            var yOffset = 5;
            var colorText = Color.White;
            if (HitboxCheck(gameState.mInputHandler.MousePosition))
            {
                colorText *= 0.25f;
            }

            foreach (var pair in gameState.mResources.ResourceDictionary)
            {
                var text = gameState.mResources.GetNameAsString(pair.Key) + ": " + pair.Value;
                var x = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(text).X / 2;
                var y = rectangle.Y + yOffset;
                spriteBatch.DrawString(mFont, text, new Vector2(x, y), colorText);
                yOffset += (int) mFont.MeasureString(text).Y + 5;
            }

            //draw string kill count
            var killtext = "Kills: " + gameState.mKillCounter;
            var killtextx = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(killtext).X / 2;
            var killtexty = rectangle.Y + yOffset;
            spriteBatch.DrawString(mFont, killtext, new Vector2(killtextx, killtexty), colorText);

            yOffset += (int)mFont.MeasureString(killtext).Y + 5;
            var capacitytext = "Kapazität: " + gameState.mCapacityHumanInSledge;
            var capacitytextx = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(capacitytext).X / 2;
            var capacitytexty = rectangle.Y + yOffset;
            spriteBatch.DrawString(mFont, capacitytext, new Vector2(capacitytextx, capacitytexty), colorText);
            // Draw string keys
            yOffset += (int)mFont.MeasureString(killtext).Y + 5;
            var keytext = "Schlüssel: " + gameState.mKeyCounter;
            var keytextx = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(keytext).X / 2;
            var keytexty = rectangle.Y + yOffset;
            spriteBatch.DrawString(mFont, keytext, new Vector2(keytextx, keytexty), colorText);
            yOffset += (int)mFont.MeasureString(killtext).Y + 5;
            var fpsText = "Fps: " + gameState.CurrentFps;
            var fpsTextx = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(keytext).X / 2;
            var fpsTexty = rectangle.Y + yOffset;
            spriteBatch.DrawString(mFont, fpsText, new Vector2(fpsTextx, fpsTexty), colorText);
            yOffset += (int)mFont.MeasureString(killtext).Y + 5;
            var playerAlive = "Einheiten: " + gameState.mPlayerAliveHumans.Count;
            var playerAlivex = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(keytext).X / 2;
            var playerAlivey = rectangle.Y + yOffset;
            spriteBatch.DrawString(mFont, playerAlive, new Vector2(playerAlivex, playerAlivey), colorText);
        }

        private bool HitboxCheck(Vector2 vector2)
        {
            var mouseRectangle = new Rectangle((int) vector2.X, (int) vector2.Y, 1, 1);
            var rectangle = new Rectangle(32, 32, 128, 256);
            return mouseRectangle.Intersects(rectangle);
        }
    }
}