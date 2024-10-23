using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
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



    public class Volume : State
    {

        public List<Component> _components;
        //private SoundEffect soundEffect;
        //private SoundEffectInstance instance;
        




        public Volume(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)

    {
        var buttonTexture = _content.Load<Texture2D>("Controls/knopf");
        var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

        var LouderButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(350, 350),
            Text = "Louder",

        };
        LouderButton.Click += LouderButton_Click;

        var SofterButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(350, 400),
            Text = "Softer",

        };
        SofterButton.Click += SofterButton_Click;

        var BackButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(350, 450),
            Text = "Back",

        };
        BackButton.Click += BackButton_Click;

        _components = new List<Component>()
            {
                LouderButton,
                SofterButton,
                BackButton,



            };

    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var Component in _components)
            Component.Draw(gameTime, spriteBatch);

    }

    public void LouderButton_Click(object sender, EventArgs e)
    {
            //_game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Volume");

            // 0.0f is silent, 1.0f is full volume
            if (SoundEffect.MasterVolume < 1.0f && SoundEffect.MasterVolume <= 0.1f)
            {
                SoundEffect.MasterVolume += 0.1f;
            }
            //else if (SoundEffect.MasterVolume <= 0.1f) 
            //{ SoundEffect.MasterVolume += 0.1f; }
            else { return; }

    }

    public void SofterButton_Click(object sender, EventArgs e)
    {
            //_game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            //Console.WriteLine("Volume");

            // 0.0f is silent, 1.0f is full volume
            //MediaPlayer.Volume -= 0.1f;
            if (SoundEffect.MasterVolume > 0.1f)
            {
                SoundEffect.MasterVolume -= 0.1f;
            }
            else { SoundEffect.MasterVolume = 0.1f;  }


            /*while (SoundEffect.MasterVolume > 0.1f)
            {
                //SoundEffect.MasterVolume -= 0.1f;
                SoundEffect.MasterVolume--;
            }*/

        }



    public void BackButton_Click(object sender, EventArgs e)
    {
        _game.ChangeState(new Sound(_game, _graphicsDevice, _content));

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



public class Sound : State
    {

        public List<Component> _components;
        public Sound(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)

        {
            var buttonTexture = _content.Load<Texture2D>("Controls/knopf");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

            var VolumeButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 350),
                Text = "Volume",

            };
            VolumeButton.Click += VolumeButton_Click;

            var On_OffButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 400),
                Text = "On/Off",

            };
            On_OffButton.Click += On_OffButton_Click;

            var BackButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(350, 450),
                Text = "Back",

            };
            BackButton.Click += BackButton_Click;

            _components = new List<Component>()
            {
                VolumeButton,
                On_OffButton,
                BackButton,



            };

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var Component in _components)
                Component.Draw(gameTime, spriteBatch);

        }

        public void VolumeButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new Volume(_game, _graphicsDevice, _content));
            





        }
        

        public void On_OffButton_Click(object sender, EventArgs e)
        {
           
            if (SoundEffect.MasterVolume == 0.0f)
                SoundEffect.MasterVolume = 1.0f;
            else
                SoundEffect.MasterVolume = 0.0f;

        }

        public void BackButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new Options(_game, _graphicsDevice, _content));
            
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