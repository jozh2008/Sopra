using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.MainMenu
{
    internal sealed class Saves : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly List<Button> mComponents;
        public Saves(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, InputHandler input) : base(game, graphicsDevice, content, input)
        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;
            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();
            float buttonHeightModifier = Math.Min(game.GetSafeLoadManager().LoadGameSaves().Count, 5)/ 2.0f - 1.0f;
            mComponents = new List<Button>();
            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + buttonHeightModifier * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;
            mComponents.Add(backButton);
            buttonHeightModifier -= 1.0f;
            int i = 0;
            foreach (GameSaveModel model in game.GetSafeLoadManager().LoadGameSaves())
            {
                i++;
                var saveButton = new Button(buttonTexture, buttonFont)
                {
                    Position = new Vector2(buttonPosX, windowMiddleY + buttonHeightModifier * mButtonHeight),
                    Text = model.mDateTime.ToString(CultureInfo.CurrentCulture)
                };
                saveButton.Click += (sender, eventArgs) =>
                {
                    mGame.GetStateManager().LoadGame(model.mSeed);
                };
                buttonHeightModifier -= 1.0f;
                mComponents.Add(saveButton);
                if (i >= 5)
                {
                    break;
                }
            }
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in mComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Menu);
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            foreach (var component in mComponents)
            {
                component.Update(gameTime);
            }
        }
    }
}
