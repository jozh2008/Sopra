﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Kitchen : Sledge
    {
        public Kitchen(Texture2D texture, int gridPosX, int gridPosY, float movementSpeed, Sledge nextSledge,Grid grid) : base(texture, gridPosX, gridPosY,movementSpeed,nextSledge,grid)

        {
            mTextureRegion = new Rectangle(0, 96, 32, 32);
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle(Rectangle.X - (int)camera.PositionPixels.X,
                Rectangle.Y - (int)camera.PositionPixels.Y + (mFieldSize - mTextureRegion.Height), //it needs to be drawn as if it were 1 square higher so it can be selected from bottom and be drawn from top (stupidfix)
                mFieldSize,
                mFieldSize * (mTextureRegion.Height) / mFieldSize);             //if the to draw sprite is2 high (like tree) it needs to be a 32*64 rectangle
            spriteBatch.Draw(mTexture, rect, mTextureRegion, color);
        }

        public Kitchen(KitchenModel model,
            GameState gameState,
            Texture2D texture2D,
            float movementSpeed,
            Grid grid) :
            base(gameState, texture2D, model, movementSpeed, grid)
        {
            mTextureRegion = new Rectangle(0, 96, 32, 32);
            gameState.mHasKueche = true;
        }
        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var sledgeModel = new KitchenModel()
            {
                X = mGridPos.X + gameState.GetGridOffset().X,
                Y = mGridPos.Y + gameState.GetGridOffset().Y,
                PreviousSledgeUuid = mPreviousSledge is null ? "none" : mPreviousSledge.Uuid.ToString(),
                Uuid = Uuid.ToString()
            };
            return sledgeModel;
        }
    }
}