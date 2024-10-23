using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;

namespace TheFrozenDesert.MainMenu
{
    internal sealed class Volume : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly List<Button> mComponents;

        public Volume(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)

        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;

            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();

            var louderButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Lauter"
            };
            louderButton.Click += LouderButton_Click;

            var softerButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 0.5f * mButtonHeight),
                Text = "Leiser"
            };
            softerButton.Click += SofterButton_Click;

            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 0.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;

            mComponents = new List<Button>
            {
                louderButton,
                softerButton,
                backButton
            };
        }


        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var button in mComponents)
            {
                button.Draw(gameTime, spriteBatch);
            }
        }

        private void LouderButton_Click(object sender, EventArgs e)
        {
            mGame.GetSoundManager().VolumeUp();
        }

        private void SofterButton_Click(object sender, EventArgs e)
        {
            mGame.GetSoundManager().VolumeDown();
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.SoundMenu);
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            foreach (var button in mComponents)
            {
                button.Update(gameTime);
            }
        }
    }


    internal sealed class Sound : State
    {
        private readonly int mButtonHeight = 73;

        private readonly int mButtonWidth = 272;
        private readonly List<Button> mComponents;

        public Sound(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)

        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;

            var buttonTexture = mContent.Load<Texture2D>("Controls/knopf");
            var buttonFont = mContent.Load<SpriteFont>("Fonts/Font");

            var volumeButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Lautstärke"
            };
            volumeButton.Click += VolumeButton_Click;

            var onOffButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 0.5f * mButtonHeight),
                Text = "An/Aus"
            };
            onOffButton.Click += On_OffButton_Click;

            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 0.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;

            mComponents = new List<Button>
            {
                volumeButton,
                onOffButton,
                backButton
            };
        }


        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var button in mComponents)
            {
                button.Draw(gameTime, spriteBatch);
            }
        }

        private void VolumeButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.VolumeMenu);
        }


        private void On_OffButton_Click(object sender, EventArgs e)
        {
            mGame.GetSoundManager().ToggleSound(); 
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Options);
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            foreach (var button in mComponents)
            {
                button.Update(gameTime);
            }
        }
    }
}