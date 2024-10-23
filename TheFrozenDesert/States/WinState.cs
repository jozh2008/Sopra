using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;
using Microsoft.Xna.Framework.Input;

namespace TheFrozenDesert.States
{
    public sealed class WinState : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
       
        private readonly Texture2D mWinScreen;
        private readonly Button mNewGameButton;

        public WinState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, InputHandler input) : base(game, graphicsDevice, content, input)
        {
            game.GetSoundManager().WinSound();

            game.GetContentManager().GetTexture("WinStateText");

            var windowMiddleY = graphicsDevice.Viewport.Height / 2;
            var windowMiddleX = graphicsDevice.Viewport.Width / 2;
            var buttonPosX = windowMiddleX - mButtonWidth / 2;
            var buttonTexture = game.GetContentManager().GetTexture("Controls/WinKnopf");
            mWinScreen = mContent.Load<Texture2D>("Controls/WinScreen");
            var buttonFont = game.GetContentManager().GetFont();
            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX + 550, windowMiddleY + 2 * mButtonHeight + 150),
                Text = "New Game"
            };
            newGameButton.Click += newGameButton_Click;
            mNewGameButton = newGameButton;
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(mWinScreen, new Rectangle(0, 0, 1300, 750), Color.AliceBlue);
            mNewGameButton.Draw(gameTime, spriteBatch);
            
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            mNewGameButton.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mGame.Exit();
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.NewGame);
        }

    }
}