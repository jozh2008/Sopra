using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheFrozenDesert
{
    public abstract class MenuComponent
    {
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }
}