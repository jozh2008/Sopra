using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;


//namespace MainMenu
namespace TheFrozenDesert.MainMenu
{
    internal sealed class GesamtStatistics : State
    {
        private readonly int mButtonHeight = 73;

        private readonly int mButtonWidth = 272 ;
        private List<Button> mComponents;

        public GesamtStatistics(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)

        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth -10 ; //2
            var buttonPosXx = windowMiddleX - mButtonWidth /40 ;

            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var parentbuttonTexture = game.GetContentManager().GetTexture("Controls/Knöpfchen");
            var buttonFont = game.GetContentManager().GetFont();
            List<GameDataModel> gameData = game.GetSafeLoadManager().LoadGameDataModelsModelsForStatistics();
            double totalTime = 0.0;
            foreach (GameDataModel model in gameData)
            {
                totalTime += model.Timer;
            }

            var numberOfGamesWonButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 0.5f * mButtonHeight),
                Text = "Anzahl der gewonnenen Spiele"
            };
            
            var giveNumberOfGamesWonButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosXx, windowMiddleY - 0.5f * mButtonHeight),
                Text = game.StatisticsModel.mNumberOfGamesWon.ToString()

            };

            
            var fastestVictoryButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Schnellster Sieg"
            };

            string fastestText = "XX:XX:XX";
            if (game.StatisticsModel.mFastestVictory > 0)
            {
                int fastestMinutes = (int)(Math.Floor(game.StatisticsModel.mFastestVictory / 60) % 60);
                int fastestHours = (int)Math.Floor(game.StatisticsModel.mFastestVictory / 3600);
                fastestText =  new DateTime(1, 1, 1, fastestHours, fastestMinutes, (int)totalTime % 60).ToLongTimeString();
            }
            var giveFastestVictoryButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosXx, windowMiddleY - 1.5f * mButtonHeight),
                Text = fastestText
            };


            var backButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(windowMiddleX - mButtonWidth / 2, windowMiddleY + 1.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;
            var playTimeButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY + 0.5f * mButtonHeight),
                Text = "Spielzeit"
            };
            
            int minutes = (int)(Math.Floor(totalTime / 60) % 60);
            int hours = (int)Math.Floor(totalTime / 3600);
            DateTime dateTime = new DateTime(1, 1, 1, hours, minutes, (int)totalTime % 60);
            var givePlayTimeButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosXx, windowMiddleY + 0.5f * mButtonHeight),
                Text = dateTime.ToLongTimeString()
            };

            mComponents = new List<Button>
            {
                numberOfGamesWonButton,
                giveNumberOfGamesWonButton,
                giveFastestVictoryButton,
                fastestVictoryButton,
                backButton,
                givePlayTimeButton,
                playTimeButton
            };
        }


        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var button in mComponents)
            {
                button.Draw(gameTime, spriteBatch);
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
            foreach (var button in mComponents)
            {
                button.Update(gameTime);
            }
        }
    }
}






