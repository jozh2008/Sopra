using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MainMenu.Controls;
using MainMenu.States;



namespace MainMenu.States
{
    public class MenuState : State 
    {
        private List<Component> _components;
        public MenuState(Game1 game ,GraphicsDevice graphicsDevice , ContentManager content):base(game,graphicsDevice,content)
        {
            var buttonTexture = mContent.Load<Texture2D>("Controls/knopf");
            var buttonFont = mContent.Load<SpriteFont>("Fonts/Font");
            var newGameButton = new Button(buttonTexture , buttonFont)
            {
                Position = new Vector2(400, 250),
                Text = "New game",

            };
            newGameButton.Click +=  newGameButton_Click; 

            var LoadGameButton = new Button(buttonTexture, buttonFont)
            {
                 Position = new Vector2(400, 300),
                 Text = "LoadGame",

            };
            LoadGameButton.Click +=  LoadGameButton_Click; 
             
            var OptionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(400,350),
                Text = "Options",

            };
            OptionsButton.Click += OptionsButton_Click;



           var StatisticsButton = new Button(buttonTexture, buttonFont)
           {
                Position = new Vector2(400, 400),
                Text = "Statistics",

           };
           StatisticsButton.Click += StatisticsButton_Click;


           var AchievementsButton = new Button( buttonTexture , buttonFont)
           {
               Position = new Vector2(400, 450),
               Text = "Achievements",

            };
           AchievementsButton.Click += AchievementsButton_Click;

            var QuiteButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(400, 500),
                Text = "Quite",

            };
           QuiteButton.Click += QuiteButton_Click;


            _components = new List<Component>()
            {
                newGameButton,
                LoadGameButton,
                OptionsButton,
                StatisticsButton,
                AchievementsButton,
                QuiteButton,

 

            };

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
           
            foreach (var Component in _components)
                Component.Draw(gameTime, spriteBatch);
            
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            mGame.ChangeState(new GameState(mGame, mGraphicsDevice, mContent)); 
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            mGame.ChangeState(new GameState(mGame, mGraphicsDevice, mContent));

            
            Console.WriteLine("LoadGame");
        }
        private void OptionsButton_Click(object sender, EventArgs e)
        {
            
            mGame.ChangeState(new GameState(mGame, mGraphicsDevice, mContent));
            Console.WriteLine("Options");
        }
        private void AchievementsButton_Click(object sender, EventArgs e)
        {
            
            mGame.ChangeState(new GameState(mGame, mGraphicsDevice, mContent));
            Console.WriteLine("Achievements");
        }
        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            
            mGame.ChangeState(new GameState(mGame, mGraphicsDevice, mContent));
            Console.WriteLine("Statistics");
        }
       
        public override void PostUpdate(GameTime gameTime)
        {


        }

        public override void Update(GameTime gameTime)
        {

        foreach (var Component in _components)
            Component.Update(gameTime);
  
        }

        private void QuiteButton_Click(object sender, EventArgs e)
        {
            mGame.Exit();
        }

    }
}
