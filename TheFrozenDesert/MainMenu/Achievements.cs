using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;
using Microsoft.Xna.Framework.Input;


namespace TheFrozenDesert.MainMenu
{
    internal sealed class Achievements : State 
    {
        //public Texture2D mBackgroundTexture;
        private readonly Texture2D mKeyKing;
        private readonly Texture2D mNotKeyKing;
        private readonly Texture2D mRaceToTheNorthPole;
        private readonly Texture2D mNotRaceToTheNorthPole;
        private readonly Texture2D mVictory;
        private readonly Texture2D mNotVictory;
        private readonly Texture2D mFirstKill;
        private readonly Texture2D mNotFirstKill;
        private readonly Texture2D mWood;
        private readonly Texture2D mNotWood;
        private readonly Texture2D mHardcore;
        private readonly Texture2D mNotHardcore;
        private readonly Texture2D mMetal;
        private readonly Texture2D mNotMetal;
        private readonly Texture2D mTenKills;
        private readonly Texture2D mNotTenKills;
        private readonly Texture2D mBackground;
        //public GraphicsDevice graphicsDevice; 



        public Achievements(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)
        {
            mBackground = content.Load<Texture2D>("Achievements_images/mBackground");
            mKeyKing = content.Load<Texture2D>("Achievements_images/KeyKing");
            mNotKeyKing = content.Load<Texture2D>("Achievements_images/NotKeyKing");
            mRaceToTheNorthPole = content.Load<Texture2D>("Achievements_images/RaceToTheNorthPole");
            mNotRaceToTheNorthPole = content.Load<Texture2D>("Achievements_images/NotRaceToTheNorthPole");
            mMetal =content.Load<Texture2D>("Achievements_images/Metal");
            mNotMetal = content.Load<Texture2D>("Achievements_images/NotMetal");
            mWood = content.Load<Texture2D>("Achievements_images/Wood");
            mNotWood = content.Load<Texture2D>("Achievements_images/NotWood");
            mVictory = content.Load<Texture2D>("Achievements_images/Victory");
            mNotVictory = content.Load<Texture2D>("Achievements_images/NotVictory");
            mTenKills = content.Load<Texture2D>("Achievements_images/TenKills");
            mNotTenKills = content.Load<Texture2D>("Achievements_images/NotTenKills");
            mFirstKill = content.Load<Texture2D>("Achievements_images/FirstKill");
            mNotFirstKill = content.Load<Texture2D>("Achievements_images/NotFirstKill");
            mHardcore = content.Load<Texture2D>("Achievements_images/Hardcore");
            mNotHardcore = content.Load<Texture2D>("Achievements_images/NotHardcore");


        }


        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            //var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            //var buttonPosX = graphicsDevice.Viewport.Width / 2 - mButtonWidth / 2;

            var buttonPosX = 415;

            spriteBatch.Draw(mBackground,
                new Rectangle(0,
                    0,
                    1280,
                    720),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mMetal ? mMetal : mNotMetal,
                new Rectangle(buttonPosX,
                    70,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mWood ? mWood : mNotWood,
                new Rectangle(buttonPosX,
                    140,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mRaceToTheNorthPole ? mRaceToTheNorthPole : mNotRaceToTheNorthPole,
                new Rectangle(buttonPosX,
                    210,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mVictory ? mVictory : mNotVictory,
                new Rectangle(buttonPosX,
                    280,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mFirstKill ? mFirstKill : mNotFirstKill,
                new Rectangle(buttonPosX,
                    350,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mTenKills ? mTenKills : mNotTenKills,
                new Rectangle(buttonPosX,
                    420,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mHardcore ? mHardcore : mNotHardcore,
                new Rectangle(buttonPosX,
                    490,
                    400,
                    70),
                Color.AliceBlue);

            spriteBatch.Draw(mGame.AchievementsModel.mKeyKing ? mKeyKing : mNotKeyKing,
                new Rectangle(buttonPosX,
                    560,
                    400,
                    70),
                Color.AliceBlue);
    
        }


        internal override void PostUpdate(GameTime gameTime)
        {
        }


        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            /*foreach (var button in mComponents)
            {
                button.Update(gameTime);
            }*/
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mGame.GetStateManager().ChangeState(StateManager.StateType.Menu);
            }
        }
    }
}
