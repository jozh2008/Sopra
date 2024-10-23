using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MainMenu.Controls;
using MainMenu.States;


namespace MainMenu
{



    public class Statistics : State
    {

        public List<Component> _components;
        public Statistics(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)

        {
            var buttonTexture = _content.Load<Texture2D>("Controls/knopf");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

            var AllStatisticsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 250),
                Text = "All Statistics",

            };
            AllStatisticsButton.Click += AllStatisticsButton_Click;

            var PlayTimeButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 300),
                Text = "Play Time",

            };
            PlayTimeButton.Click += PlayTimeButton_Click;

            var DistinguishedVictoryButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 350),
                Text = "Distinguished Victory",

            };
            DistinguishedVictoryButton.Click += DistinguishedVictoryButton_Click;


            var FastestVictoryButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 400),
                Text = "Fastest Victory",

            };
            FastestVictoryButton.Click += FastestVictoryButton_Click;


            var BackButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350,450),
                Text = "Back",

            };
            BackButton.Click += BackButton_Click;

            _components = new List<Component>()
            {
                AllStatisticsButton,
                PlayTimeButton,
                DistinguishedVictoryButton,
                FastestVictoryButton,
                BackButton,



            };

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var Component in _components)
                Component.Draw(gameTime, spriteBatch);

        }

        public void AllStatisticsButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Sound");
        }

        public void PlayTimeButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Techdemo");
        }

        public void DistinguishedVictoryButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Sound");
        }

        public void FastestVictoryButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Sound");
        }

        public void BackButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Back");
        }

        public override void PostUpdate(GameTime gameTime)
        {


        }

        public override void Update(GameTime gameTime)
        {

            foreach (var Component in _components)
                Component.Update(gameTime);

        }
    }
}
