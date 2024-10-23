using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TestMonogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _background;
        private Texture2D _unilogo;
        private float _dimension;
        private float _rotation;
        private float _radius;
        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        private bool IsInCircle(float x, float y)
        {
            
            x -= _unilogo.Bounds.X + GraphicsDevice.Viewport.Bounds.Width / 2;
            y -= _unilogo.Bounds.Y + GraphicsDevice.Viewport.Bounds.Height / 2;
            return x * x + y * y <= _radius * _radius;
        }
        protected override void Initialize()
        {
            _dimension = 0.15f;
            _rotation = 0f;
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _background = Content.Load<Texture2D>("hintergrund");
            _unilogo = Content.Load<Texture2D>("Unilogo");
            _radius = (_unilogo.Width * _dimension)/2.0f;
            mSoundHit = Content.Load<SoundEffect>("Logo_hit");
            mSoundMiss = Content.Load<SoundEffect>("Logo_miss");


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (mouseState.LeftButton == ButtonState.Pressed )
            {
                if (IsInCircle(mouseState.X, mouseState.Y)){
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
                
            }
            // TODO: Add your update logic here
            _rotation += 0.01f;
            _rotation = _rotation % 360;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            float x = (float)(_radius * Math.Cos(_rotation) - _radius * Math.Sin(_rotation));
            float y = (float)(_radius * Math.Cos(_rotation) + _radius * Math.Sin(_rotation));

            _spriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height), Color.White);
            _spriteBatch.Draw( _unilogo,  new Vector2((GraphicsDevice.Viewport.Bounds.Width/2)+x, (GraphicsDevice.Viewport.Bounds.Height/2)+y), null, Color.White ,  _rotation, new Vector2(_unilogo.Width/2, _unilogo.Height/2), _dimension, SpriteEffects.None, 0);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
