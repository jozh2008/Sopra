using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;

namespace TheFrozenDesert.States
{
    public sealed class MenuState : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly List<MenuComponent> mComponents;

        public MenuState(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)
        {
            // music
            game.GetSoundManager().MainMenuSound();

            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;
            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();
            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 3 * mButtonHeight),
                Text = "Neues Spiel"
            };
            newGameButton.Click += newGameButton_Click;

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 2 * mButtonHeight),
                Text = "Spiel laden"
            };
            loadGameButton.Click += LoadGameButton_Click;

            var optionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1 * mButtonHeight),
                Text = "Optionen"
            };
            optionsButton.Click += OptionsButton_Click;


            var statisticsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY),
                Text = "Statistiken"
            };
            statisticsButton.Click += StatisticsButton_Click;


            var achievementsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + mButtonHeight),
                Text = "Achievements"
            };
            achievementsButton.Click += AchievementsButton_Click;

            var quitButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 2 * mButtonHeight),
                Text = "Beenden"
            };
            quitButton.Click += QuiteButton_Click;


            mComponents = new List<MenuComponent>
            {
                newGameButton,
                loadGameButton,
                optionsButton,
                statisticsButton,
                achievementsButton,
                quitButton
            };
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in mComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.NewGame);
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.LoadGame);


            Console.WriteLine("Spiel laden");
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.OptionsMenu);
            Console.WriteLine("Optionen");
        }

        private void AchievementsButton_Click(object sender, EventArgs e)
        {
            // TODO add right state
            mGame.GetStateManager().ChangeState(StateManager.StateType.AchievementsMenu);
            //Console.WriteLine("Achievements");
        }

        private void StatisticsButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.GesamtStatisticsMenu); 
            

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

        private void QuiteButton_Click(object sender, EventArgs e)
        {
            mGame.Exit();
        }
    }
}