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
    internal sealed class PauseMenuOptions : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly List<Button> mComponents;

        public PauseMenuOptions(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)

        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;

            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();

            var soundButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Sound"
            };
            soundButton.Click += SoundButton_Click;

            var techdemoButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 0.5f * mButtonHeight),
                Text = "Techdemo"
            };
            techdemoButton.Click += TechdemoButton_Click;

            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 0.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;

            mComponents = new List<Button>
            {
                soundButton,
                techdemoButton,
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

        private void SoundButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.SoundMenu);
            //Console.WriteLine("Sound");
        }

        private void TechdemoButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Techdemo);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.PauseMenu);
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