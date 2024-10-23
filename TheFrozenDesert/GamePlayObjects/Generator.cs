using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public class Generator : AbstractGameObject
    {
        private float mClaimProgress;
        private readonly GamePlayObjectInfoBar mClaimBar;
        public Generator(Texture2D texture, int gridPosX, int gridPosY, Grid grid) : base(texture,
            gridPosX,
            gridPosY,
            grid)
        {
            mClaimProgress = 0;
            mClaimBar = new GamePlayObjectInfoBar(this, 0, Color.Orange);
            mTextureRegion = new Rectangle(0, 32, 32, 32);
            mClaimBar.SetPercent(0);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            if (mClaimProgress >= 100.0f)
            {
                gameState.Win();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
            mClaimBar.Draw(spriteBatch, camera);
        }

        public override void Select(List<AbstractGameObject> previouslySelectedObjects)
        {
            base.Select(previouslySelectedObjects);
            foreach (AbstractGameObject abstractGameObject in previouslySelectedObjects)
            {
                if (abstractGameObject is Human {Faction: EFaction.Player} human)
                {
                    human.SetGeneratorAsTarget(this);
                }
            }
        }

        public void AddProgress(float amount)
        {
            mClaimProgress += amount;
            mClaimBar.SetPercent((int)Math.Min(mClaimProgress, 100.0f));
        }
    }
}