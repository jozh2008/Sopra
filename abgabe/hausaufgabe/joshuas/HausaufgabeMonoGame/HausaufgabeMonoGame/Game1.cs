using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
namespace HausaufgabeMonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch; 
        private Texture2D _texture;
        private Texture2D _background;
        private List<SoundEffect> soundEffects;
        private MouseState mouseState;
        private Rectangle destinationRectangle;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            soundEffects = new List<SoundEffect>();
            destinationRectangle = new Rectangle(50, 50, 200, 200);
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
            _texture = this.Content.Load<Texture2D>("Unilogo");
            _background = this.Content.Load<Texture2D>("Background");

            soundEffects.Add(Content.Load<SoundEffect>("Logo_hit"));
            soundEffects.Add(Content.Load<SoundEffect>("Logo_miss"));
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            mouseState = Mouse.GetState();
            Rectangle mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseRectangle.Intersects(destinationRectangle))
                {
                    soundEffects[0].Play();
                }
                else
                {
                    soundEffects[1].Play();
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(0, 0, 800, 480),Color.White);
            _spriteBatch.Draw(_texture, destinationRectangle, color:Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
