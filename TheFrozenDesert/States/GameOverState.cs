using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Input;

namespace TheFrozenDesert.States
{
    internal sealed class GameOverState : State
    {
        private readonly int mButtonHeight = 73;
        private readonly int mButtonWidth = 272;
        private readonly Button mQuitButton;
        private readonly Texture2D mGameOverText;
        private readonly int mWindowMiddleY;
        private readonly int mWindowMiddleX;
        public GameOverState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, InputHandler input) : base(game, graphicsDevice, content, input)
        {
            game.GetSoundManager().GameOverSound();
            mGameOverText = game.GetContentManager().GetTexture("GameOverText");
            mWindowMiddleY = graphicsDevice.Viewport.Height / 2;
            mWindowMiddleX = graphicsDevice.Viewport.Width / 2;
            var buttonPosX = mWindowMiddleX - mButtonWidth / 2;
            var buttonTexture = game.GetContentManager().GetTexture("Controls/knopf");
            var buttonFont = game.GetContentManager().GetFont();
            var quitButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonPosX, mWindowMiddleY + 2 * mButtonHeight),
                Text = "Quit"
            };
            quitButton.Click += QuitButton_Click;
            mQuitButton = quitButton;
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mQuitButton.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(mGameOverText, new Rectangle(new Point(mWindowMiddleX - mGameOverText.Width/2, mWindowMiddleY), new Point(mGameOverText.Width, mGameOverText.Height)), Color.White);
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            mQuitButton.Update(gameTime);
        }
        private void QuitButton_Click(object sender, EventArgs e)
        {
            mGame.GetStateManager().ChangeState(StateManager.StateType.Menu);
        }
    }
}
