
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;



namespace TheFrozenDesert.MainMenu
{
    internal sealed class InGameStatistics : State
    {
        private readonly int mButtonHeight = 73;

        private readonly int mButtonWidth = 272;
        private readonly List<Button> mComponents;

        public InGameStatistics(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input) : base(game, graphicsDevice, content, input)

        {
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var buttonPosX = windowMiddleX - mButtonWidth -10 ;
            //var buttonPosX = windowMiddleX - mButtonWidth; //2
            
            var buttonPosXx = windowMiddleX - mButtonWidth / 45;

            var parentbuttonTexture = game.GetContentManager().GetTexture("Controls/Knöpfchen");
            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();

            var allResourcesButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 2.5f * mButtonHeight),
                Text = "Alle Resourcen"
            };
            
            GameState gameState = mGame.GetStateManager().GameState;
            var giveMeatButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY -0.5f * mButtonHeight),
                Text = "Fleisch: " + gameState.mResources.Get(ResourceType.Meat)
            };


            var giveWoodButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY - 1.5f * mButtonHeight),
                Text = "Holz: " + gameState.mResources.Get(ResourceType.Wood)
            };
            

            var giveMetalButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, windowMiddleY +0.5f * mButtonHeight),
                Text = "Metall: " + gameState.mResources.Get(ResourceType.Metall)
            };

            var killCount = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosXx, windowMiddleY - 2.5f * mButtonHeight),
                Text = "Anzahl der getöteten Gegner"
            };

            var giveKillCount = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosXx, windowMiddleY - 1.5f * mButtonHeight),
                Text = gameState.mKillCounter.ToString()
            };
            
            var backButton = new Button(parentbuttonTexture, buttonFont)
            {
                Position = new Vector2(windowMiddleX - mButtonWidth / 2, windowMiddleY + 2.5f * mButtonHeight),
                Text = "Zurück"
            };
            backButton.Click += BackButton_Click;



            mComponents = new List<Button>
            {
                killCount,
                giveKillCount,
                backButton,
                allResourcesButton,
                giveWoodButton,
                giveMetalButton,
                giveMeatButton
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
