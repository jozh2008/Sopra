using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheFrozenDesert.Controls;
using TheFrozenDesert.States;

namespace TheFrozenDesert
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private State _currentState;
        private State _nextState;
        


        public void ChangeState(State state)
        {

            _nextState =state;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentState = new MenuState(this, graphics.GraphicsDevice , Content);

            // TODO: use this.Content to load your game content here
            _backgroundTexture = Content.Load<Texture2D>("Controls/Background");
        }

        protected override void Update(GameTime gameTime)
        {

            if(_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
             GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0 ,graphics.PreferredBackBufferWidth,
            graphics.PreferredBackBufferHeight), Color.AliceBlue);

           
            _currentState.Draw(gameTime, _spriteBatch);
            _spriteBatch.End(); 


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

