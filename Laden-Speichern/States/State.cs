using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TheFrozenDesert.States
{
    public abstract class State
    {
        #region Fields
        protected ContentManager _content;
        protected GraphicsDevice _graphicsDevice;
        protected Game1 _game;
        #endregion
        #region Methods
        public abstract void Draw(GameTime gameTime, SpriteBatch spritebatch);
        public abstract void PostUpdate(GameTime gameTime);
        public State(Game1 game , GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content; 

        }
        public abstract void Update(GameTime gameTime);
        #endregion
    }
}