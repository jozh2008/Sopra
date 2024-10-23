using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;

namespace TheFrozenDesert.States
{
    public sealed class PauseState : State
    {
        
  
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly List<MenuComponent> mComponents;


        public PauseState(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)
        //public PauseState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content, input , window)
        {

            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;
            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();
            game.GetSoundManager().PauseMenuSound();
            var saveButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 2.5f * mButtonHeight),
                Text = "Speichern"
            };
            saveButton.Click += SaveButton_Click;

            var startMenuButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Startmenü"
            };
            startMenuButton.Click += StartMenuButton_Click;

            var optionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 0.5f * mButtonHeight),
                Text = "Optionen"
            };
            optionsButton.Click += OptionsButton_Click;


            var statisticsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 0.5f * mButtonHeight),
                Text = "Statistiken"
            };
            statisticsButton.Click += StatisticsButton_Click;


            var achievementsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 1.5f * mButtonHeight),
                Text = "Achievements"
            };
            achievementsButton.Click += AchievementsButton_Click;

            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 2.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;


            mComponents = new List<MenuComponent>
            {
                saveButton,
                startMenuButton,
                optionsButton,
                statisticsButton,
                achievementsButton,
                backButton
            };
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in mComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
        }

        private void StartMenuButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Menu);
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.PauseMenuOptions);
        }

        private void AchievementsButton_Click(object sender, EventArgs e)
        {
            //_game.ChangeState(new Achievements(_game, _graphicsDevice, _content));
            mGame.GetStateManager().ChangeState(StateManager.StateType.AchievementsMenu);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Game);
        }

        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.InGameStatisticsMenu);

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

            //mCurrentSong.PlaySong();
        }
    }
}