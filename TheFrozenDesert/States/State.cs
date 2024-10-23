using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Input;

namespace TheFrozenDesert.States
{
    public abstract class State
    {
        #region Fields

        protected readonly ContentManager mContent;
        protected readonly GraphicsDevice mGraphicsDevice;
        public readonly Game1 mGame ;
        public readonly InputHandler mInputHandler;

        #endregion

        #region Methods

        internal abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        internal abstract void PostUpdate(GameTime gameTime);

        protected State(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input)
        {
            mGame = game;
            mGraphicsDevice = graphicsDevice;
            mContent = content;
            mInputHandler = input;
        }

        internal abstract void Update(GameTime gameTime, Game1.Managers managers);

        internal Game1 GetGame()
        {
            return mGame;
        }
        #endregion
    }
}