using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class SnowTile : AbstractGameObject
    {
        private int mHealth = 100;
        private readonly float mDamageFactor;

        private readonly bool mContainsKey;

        private readonly GamePlayObjectInfoBar mHealthBar;

        internal SnowTile(Texture2D texture, int gridPosX, int gridPosY, Grid grid, bool containsKey, float damageFactor) : base(texture, gridPosX, gridPosY, grid)
        {
            mHealthBar = new GamePlayObjectInfoBar(this, +30, Color.Red);
            mContainsKey = containsKey;
            mDamageFactor = damageFactor;
        }

        internal SnowTile(Texture2D texture, SnowTileModel model, Grid grid, float damageFactor) : base(texture, model.GridCurrentPositionX, model.GridCurrentPositionY, grid)
        {
            mHealthBar = new GamePlayObjectInfoBar(this, +30, Color.Red);
            mContainsKey = model.ContainsKey;
            mDamageFactor = damageFactor;
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new SnowTileModel()
            {
                ContainsKey = mContainsKey,
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y
            };
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            UpdateHealth(gameState);
            mHealthBar.SetPercent(mHealth);
        }

        private void UpdateHealth(GameState gameState)
        {
            if (mHealth <= 0)
            {
                gameState.mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
                if (mContainsKey)
                {
                    gameState.mGrid.AddToGrid(mGridPos.X, mGridPos.Y, gameState.GamePlayObjectFactory.CreateKey(mGridPos.X, mGridPos.Y));
                }
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
            if (mHealth < 100)
            {
                mHealthBar.Draw(spriteBatch, camera);
            }
        }

        internal int Damage(int attack)
        {
            var health = (int) (mHealth - attack * mDamageFactor);
            mHealth = health <= 0 ? 0 : health;

            return mHealth;
        }

        internal bool IsDestroyed()
        {
            return mHealth <= 0;
        }
    }
}