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



    public class Options : State
    {

        public List<Component> _components;
        public Options(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)

        {
            var buttonTexture = _content.Load<Texture2D>("Controls/knopf");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

            var SoundButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 250),
                Text = "Sound",

            };
            SoundButton.Click += SoundButton_Click;

            var TechdemoButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 300),
                Text = "Techdemo",

            };
            TechdemoButton.Click += TechdemoButton_Click;

            var BackButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 350),
                Text = "Back",

            };
            BackButton.Click += BackButton_Click;

            _components = new List<Component>()
            {
                SoundButton,
                TechdemoButton,
                BackButton,



            };

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var Component in _components)
                Component.Draw(gameTime, spriteBatch);

        }

        public void SoundButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new Sound(_game, _graphicsDevice, _content));
            //Console.WriteLine("Sound");
        }

        public void TechdemoButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            Console.WriteLine("Techdemo");
        }

        public void BackButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            //Console.WriteLine("BackButton");


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
