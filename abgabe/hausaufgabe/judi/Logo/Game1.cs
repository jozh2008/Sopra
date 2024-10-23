using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D Unilogo;
        private Texture2D Backround;
        private SoundEffect hit;
        private SoundEffect miss;
        private Vector2 unilogocoordinate = new Vector2(200, 200);
        private int unilogocoordinatex;
        private int unilogocoordinatey;
        private int angle;
        private readonly int speed = 200;
        private bool mausevent;
        


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1400;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Backround = Content.Load<Texture2D>("Background");
            Unilogo = Content.Load<Texture2D>("Unilogo");            
            hit = Content.Load<SoundEffect>("Logo_hit");
            miss = Content.Load<SoundEffect>("Logo_miss");
        }

        private void Logo(GameTime Time)
        {
            int time = (int)(speed * Time.ElapsedGameTime.TotalSeconds);
            int rotation_one = (int)(150 * Math.Sin(Math.PI * angle / 360));
            int rotation_two = (int)(150 * Math.Cos(Math.PI * angle / 360));

            if(360 > angle){
                
                angle += time;
            }
            else
            {
                 angle = -360 ;
            }

            unilogocoordinatex=  rotation_two +_graphics.PreferredBackBufferWidth / 4 ;
            unilogocoordinatey=  rotation_one +_graphics.PreferredBackBufferHeight / 4;
            unilogocoordinate.X = unilogocoordinatex + 300;
            unilogocoordinate.Y = unilogocoordinatey + 200;

        }

        private void Mouse()
        {
            MouseState maus = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        

            else if (maus.LeftButton == ButtonState.Pressed && mausevent == false)
            {
                mausevent = true;
                if (Vector2.Distance(maus.Position.ToVector2(), unilogocoordinate)  < 110)
                {
                    hit.Play();
                }
                else
                {
                    miss.Play();
                }
            }
            else if (maus.LeftButton == ButtonState.Released && mausevent)
            {
                mausevent = false;
            }
        }
        protected override void Update(GameTime Time)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            Logo(Time);
            Mouse();
            base.Update(Time);
        }

        protected override void Draw(GameTime Time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(Backround, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight), Color.AliceBlue);
            spriteBatch.Draw(Unilogo, new Rectangle((int)unilogocoordinatex + 200 , (int)unilogocoordinatey + 100 , 200, 200), Color.AliceBlue);
            spriteBatch.End();
            base.Draw(Time);
        }
    }
}
