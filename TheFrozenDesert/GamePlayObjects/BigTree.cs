using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class BigTree : AbstractGameObject
    {
        private readonly Vector2 mTextureSize = new Vector2(mFieldSize * 3, mFieldSize * 4);
        private int mHealth;
        private readonly double mDamageFactor;
        private readonly GamePlayObjectInfoBar mHealthbar;

        private readonly String mMessage;
        private readonly GamePlayObjectFactory mGamePlayObjectFactory;
        private readonly GameState mGameState;

        internal BigTree(Texture2D texture, int gridPosX, int gridPosY, Grid grid, GameState gamestate, double damageFactor, String message) : base(texture, gridPosX, gridPosY, grid)
        {
            mMessage = message;
            mDamageFactor = damageFactor;
            mGamePlayObjectFactory = gamestate.GamePlayObjectFactory;
            mGameState = gamestate;
            
            mTextureRegion = new Rectangle(0,0,mTexture.Width, mTexture.Height);
            
            mHealth = 100;
            mHealthbar = new GamePlayObjectInfoBar(this, mFieldSize * 3, Color.Green);
            mHealthbar.SetScale(3);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            mHealthbar.SetPercent(mHealth);
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            //base.Draw(gameTime, spriteBatch, camera);
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle((int)mPosition.X - (int)camera.PositionPixels.X, (int)mPosition.Y - (int)camera.PositionPixels.Y, (int)mTextureSize.X, (int)mTextureSize.Y);
            spriteBatch.Draw(mTexture, rect, mTextureRegion, color);
            if (mHealth < 100)
            {
                mHealthbar.Draw(spriteBatch, camera);
            }
        }

        private void DeadRoutine()
        {
            mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
            mGamePlayObjectFactory.CreateSign(mGridPos.X, mGridPos.Y, mMessage);
            mGameState.mManagers.mSoundManager.BigTree();

        }

        internal bool Damage(int damage)
        {
            var newHealth = (int) (mHealth - damage * mDamageFactor);
            if (newHealth > 0)
            {
                mHealth = newHealth;
                return false;
            }
            mHealth = 0;
            DeadRoutine();
            return true;
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new BigTreeModel()
            {
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y
            };
        }
    }

    internal sealed class BigTreePart : AbstractGameObject
    {
        private readonly BigTree mMainTree;

        internal BigTreePart(Texture2D texture, int gridPosX, int gridPosY, Grid grid, BigTree mainTree) : base(texture, gridPosX, gridPosY, grid)
        {
            mMainTree = mainTree;
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            if (mSelected)
            {
                gameState.mGame.GetSelectionManager().SelectObject(mMainTree);
            }
        }

        internal bool Damage(int damage)
        {
            return mMainTree.Damage(damage);
        }
    }
}