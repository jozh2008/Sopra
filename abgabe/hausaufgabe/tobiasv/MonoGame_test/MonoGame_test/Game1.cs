using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MonoGame_test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch mSpriteBatch;

        private Texture2D mTextureLogo;
        private Vector2 mPositionLogo = new Vector2(100, 100);
        private int mAngleLogo;
        private int mRotationSpeedLogo = 150;   // Speed in degree per Second
        private bool mMousePressed;
        
        private Texture2D mTextureBackround;
        
        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            mTextureBackround = Content.Load<Texture2D>("Background");
            mTextureLogo = Content.Load<Texture2D>("Unilogo");
            
            // Load Sounds
            mSoundHit = Content.Load<SoundEffect>("Logo_hit");
            mSoundMiss = Content.Load<SoundEffect>("Logo_miss");
        }
        
        // update functions
        private void UpdateLogoPosition(GameTime gameTime)
        {
            int centerx = _graphics.PreferredBackBufferWidth / 2;
            int centery = _graphics.PreferredBackBufferHeight / 2;
            
            if (mAngleLogo <= -360)
            {
                mAngleLogo = 0;
            }
            else
            { 
                mAngleLogo -= (int)(mRotationSpeedLogo * gameTime.ElapsedGameTime.TotalSeconds);
            }

            double radian = Math.PI * mAngleLogo / 180;
            mPositionLogo.X = centerx + (int)(200 * Math.Sin(radian));
            mPositionLogo.Y = centery + (int)(200 * Math.Cos(radian));
        }

        private void UpdateClickEvent()
        {
            MouseState mmouse = Mouse.GetState();
            if (mmouse.LeftButton == ButtonState.Pressed && mMousePressed == false)
            {
                mMousePressed = true;
                if (Vector2.Distance(mmouse.Position.ToVector2(), mPositionLogo) < 150)
                {
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
            }
            else if (mmouse.LeftButton == ButtonState.Released && mMousePressed)
            {
                mMousePressed = false;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            UpdateLogoPosition(gameTime);
            UpdateClickEvent();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mTextureBackround, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight), Color.White);
            mSpriteBatch.Draw(mTextureLogo, new Rectangle((int)mPositionLogo.X -150, (int)mPositionLogo.Y - 150, 300, 300), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}