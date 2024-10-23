using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;

namespace AberLogo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _texture_logo;
        private Texture2D _texture_background;
        private Vector2 _position_logo;
        float _scale;
        Vector2 _origin;
        float _rotation = 0f;
        MouseState mouse;
        int _sound_pause;

        SoundEffect _soundeffect_hit;
        SoundEffect _soundeffect_miss;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _texture_logo = Content.Load<Texture2D>("Unilogo");
            _texture_background = Content.Load<Texture2D>("Background");

            _position_logo = new Vector2(GraphicsDevice.Viewport.Bounds.Width/2,GraphicsDevice.Viewport.Bounds.Height/2);
            _scale = 0.2f;
            _origin = new Vector2( _texture_logo.Width/2, _texture_logo.Height/2);

            _soundeffect_hit = this.Content.Load<SoundEffect>("Logo_hit");
            _soundeffect_miss = this.Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            mouse = Mouse.GetState();

            if(_sound_pause != 0)
            {
                _sound_pause -= 1;
            }

            if (mouse.LeftButton == ButtonState.Pressed && _sound_pause == 0)
            {
                
                if(Math.Sqrt(Math.Pow(mouse.X - _position_logo.X,2)+Math.Pow(mouse.Y - _position_logo.Y,2)) <= ((_texture_logo.Height*_scale)/2f)) 
                {
                    _soundeffect_hit.Play();
                }
                else
                {
                    _soundeffect_miss.Play();
                }
                _sound_pause = 20;

            }
            
            _rotation += 0.01f;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height), Color.White);
            _spriteBatch.Draw(texture: _texture_logo, position: _position_logo, sourceRectangle: null, color: Color.White*0.7f, rotation: _rotation, origin: _origin,scale: _scale, effects: SpriteEffects.None, layerDepth: 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
