using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestMonogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch mSpriteBatch;

        private Texture2D mLogoTexture;
        private Texture2D mBackgroundTexture;

        private SoundEffect mHitSound;
        private SoundEffect mMissSound;

        private const float RotationSpeed = 0.05f;

        private Vector2 mLogoPosition = new Vector2(640, 256);

        private MouseState mMouse;
        private bool mMousePressed = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mLogoTexture = Content.Load<Texture2D>("Unilogo");
            mBackgroundTexture = Content.Load<Texture2D>("Background");
            mHitSound = Content.Load<SoundEffect>("Logo_hit");
            mMissSound = Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mMouse = Mouse.GetState();
            if (mMouse.LeftButton == ButtonState.Pressed && mMousePressed == false)
            {
                mMousePressed = true;
                if (Vector2.Distance(mMouse.Position.ToVector2(), mLogoPosition) < mLogoTexture.Bounds.Width / 8f)
                {
                    mHitSound.Play();
                }
                else
                {
                    mMissSound.Play();
                }
            }
            if (mMouse.LeftButton == ButtonState.Released)
            {
                mMousePressed = false;
            }

            System.Console.WriteLine(RotationSpeed);
            float tempX = mLogoPosition.X;
            mLogoPosition.X = (float) Math.Cos(RotationSpeed) * (mLogoPosition.X - 640) - (float) Math.Sin(RotationSpeed) * (mLogoPosition.Y - 512) + 640;
            mLogoPosition.Y = (float) Math.Sin(RotationSpeed) * (tempX - 640) + (float) Math.Cos(RotationSpeed) * (mLogoPosition.Y - 512) + 512;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundTexture, new Vector2(0, 0), Color.White);
            mSpriteBatch.Draw(mLogoTexture, new Vector2(mLogoPosition.X - mLogoTexture.Bounds.Width / 8f, mLogoPosition.Y - mLogoTexture.Bounds.Height / 8f), null, Color.White, 0f, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
