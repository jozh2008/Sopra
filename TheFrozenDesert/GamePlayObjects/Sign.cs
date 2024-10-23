using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Sign : AbstractGameObject
    {
        internal readonly string mMessage;
        

        private bool mShowMessage;


        internal Sign(Texture2D texture, Texture2D textureGui, SpriteFont spriteFont, int gridPosX, int gridPosY, Grid grid, String message) : base(texture, gridPosX, gridPosY, grid)
        {
            mMessage = message;
            mGui = new SignGui(this, textureGui, spriteFont);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        { 
            if (mSelected)
            {
                if (gameState.mInputHandler.MouseClickRight)
                {
                    mShowMessage = !mShowMessage;
                }
            }
            else
            {
                mShowMessage = false;
            }
        }



    }
}