using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Wolf : AbstractMoveableObject
    {
        private readonly GamePlayObjectInfoBar mHealthBar;
        private bool mIsDead;
        
        // timing
        private int mLastAttack;
        private readonly int mAttackTime;
        private int mLastRegeneration;
        private readonly int mRegenerationTime;
        private bool mAttacking;
        
        // Health
        private readonly int mHealthRegenerationrate;    // how much the wolf regenerates within one second
        // Attacking
        private readonly int mAttackRadius;      // defines how big the square is in which the wolf attacks a human
        private readonly int mDamage;            // defines how much damage a wolf does per hit
        private readonly int mEarningValue;

        private EFaction mLastHitFaction;       // defines which person hit the wolf as the last (for getting meet or not)o

        private WolfPack mWolfPack;

        internal Guid Uuid { get;}

        public Wolf(Texture2D texture,
                                int gridPosX,
                                int gridPosY,
                                float movementSpeed,
                                Grid grid, int health, int healthRegenerationrate, int attackRadius, int damage, int earningValue, WolfPack wolfPack) : base(texture, gridPosX, gridPosY, movementSpeed, grid)
        {
            Health = health;
            mHealthRegenerationrate = healthRegenerationrate;
            mAttackRadius = attackRadius;
            mDamage = damage;
            mEarningValue = earningValue;
            mAttackTime = 1;
            mRegenerationTime = 5;
            mHealthBar = new GamePlayObjectInfoBar(this, -5, Color.LightGreen);
            mLastHitFaction = EFaction.Neutral;
            mWolfPack = wolfPack;
            Uuid = new Guid();
        }

        internal Wolf(Texture2D texture,
            WolfModel model,
            float movementSpeed,
            Grid grid) : base(texture,
            model.GridCurrentPositionX,
            model.GridCurrentPositionY,
            movementSpeed,
            grid)
        {
            Health = model.Health;
            mHealthRegenerationrate = model.HealthRegenerationrate;
            mAttackRadius = model.AttackRadius;
            mDamage = model.Damage;
            mAttackTime = 1;
            mRegenerationTime = 5;
            mHealthBar = new GamePlayObjectInfoBar(this, -5, Color.LightGreen);
            mLastHitFaction = EFaction.Neutral;
            Uuid = Guid.Parse(model.Uuid);
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new WolfModel()
            {
                AttackRadius = mAttackRadius,
                Damage = mDamage,
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y,
                Health = Health,
                HealthRegenerationrate = mHealthRegenerationrate,
                Uuid = Uuid.ToString()
            };
        }

        /* *********************************************************************
         *  Functions to interact with Wolf
         * *********************************************************************/
        internal void Damage(int damage, EFaction faction)
        {
            mLastHitFaction = faction;
            if (Health >= damage)
            {
                Health -= damage;
            }
            else
            {
                Health = 0;
                mIsDead = true;
            }
        }

        internal int Health { get; private set; }

        internal void AddToPack(WolfPack wolfPack)
        {
            mWolfPack = wolfPack;
        }
/* *********************************************************************
 *  Drawing logik
 * *********************************************************************/
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            // TODO animator for base.Draw(gameTime, spriteBatch, camera);
            // this draw logik is only there because other wise the hole sprite sheet would be drawn
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle(Rectangle.X - (int) camera.PositionPixels.X,
                Rectangle.Y - (int) camera.PositionPixels.Y,
                mFieldSize,
                mFieldSize);
            spriteBatch.Draw(mTexture, rect, new Rectangle(0,0,32,32), color);
            
            mHealthBar.Draw(spriteBatch, camera);
        }
/* *********************************************************************
 *  Updating Logik
 * *********************************************************************/

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            UpdateHealth(gameTime, gameState);
            base.Update(gameTime, grid, gameState, soundManager);
            CheckSurrounding(gameTime, gameState);
        }
        
        // Health logik
        private void UpdateHealth(GameTime gameTime, GameState gameState)
        {
            mHealthBar.SetPercent(Health);
            if (Health <= 0)
            {
                mIsDead = true;
            }
            if (mIsDead)
            {
                // TODO implement animation dead
                gameState.mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
                if (mLastHitFaction == EFaction.Player)
                {
                    gameState.mResources.Increase(ResourceType.Meat, mEarningValue);
                }
                else if (mLastHitFaction == EFaction.Enemy)
                {
                    gameState.mEnemyResources.Increase(ResourceType.Meat, mEarningValue);
                }

            }
            else
            {
               RegenerateHealth(gameTime);
            }
        }

        private void RegenerateHealth(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Seconds - mLastRegeneration < mRegenerationTime) 
            { 
                return;
            }

            mLastRegeneration = gameTime.TotalGameTime.Seconds;
            var newHealth = Health + mHealthRegenerationrate;
            Health = newHealth > 100 ? 100 : newHealth;
        }
        private void CheckSurrounding(GameTime gameTime, GameState gameState)
        {
            AttackHumanInArea(gameTime, gameState);
            if (!mAttacking) // only return to pack if not attacking currently
            {
                MoveToWolvesInArea();
            }
        }

        private void AttackHumanInArea(GameTime gameTime, GameState gameState)
        {
            // check if last attack was long enough ago or if wolf is moving
            if (gameTime.TotalGameTime.Seconds - mLastAttack < mAttackTime || IsMoving())
            {
                return;
            }
            
            // iterate trough fields in Attack radius
            for (var x = mGridPos.X - mAttackRadius; x <= mGridPos.X + mAttackRadius; x++)
            {
                for (var y = mGridPos.Y - mAttackRadius; y <= mGridPos.Y + mAttackRadius; y++)
                {
                    if (x >= 0 && x < mGrid.Size.X && y >= 0 && y < mGrid.Size.Y)
                    {
                        var gameObject = gameState.mGrid.GetAbstractGameObjectAtGridPosition(new Vector2(x, y));
                        if (gameObject is EmptyObject || gameObject is ResourceObject)
                        {
                            continue;
                        }
                        if (gameObject is Human human)
                        {
                            mAttacking = true;
                            // walk to human
                            SetGridTargetToNextFreePosition(x, y);
                            // attack found h
                            // only attack if human is next to Wolf
                            if (CheckNextTo(human))
                            {
                                human.DamageHuman(mDamage, gameState, true);
                                mLastAttack = gameTime.TotalGameTime.Seconds;
                            }

                            return; // stop iterating
                        }
                    }
                }
            }

            mAttacking = false;
        }

        private void MoveToWolvesInArea()
        {
            if (mWolfPack != null) // otherwise wolf is not in a pack
            {
                var area = mWolfPack.WolfArea;
                // TODO walk to middle field of area if its empty
                // TODO check if field is empty
                SetGridTargetToNextFreePosition(area.X, area.Y);

            }

        }

        private void SetGridTargetToNextFreePosition(int targetX, int targetY)
        {
            // TODO logik if there is a tree
            var direction = new Vector2(mGridPos.X - targetX, mGridPos.Y - targetY);
            direction.Normalize();

            SetGridTarget((int)Math.Round(targetX + direction.X), (int)Math.Round(targetY + direction.Y));
        }
        private bool CheckNextTo(AbstractGameObject gameObject)
        {
            var objPosition = gameObject.GetGridPos();
            var xDistance = Math.Abs(mGridPos.X - objPosition.X);
            var yDistance = Math.Abs(mGridPos.Y - objPosition.Y);

            if (xDistance <= 1 && yDistance <= 1)
            {
                return true;
            }
            return false;
        }
    }
}