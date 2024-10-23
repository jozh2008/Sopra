using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.AI;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.Input;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Archer : Human
    {
        private readonly int mArrowRange;

        private readonly List<Arrow> mArrowList = new List<Arrow>();

        private Bow Bow { get; set; }
        private string mBowType;
        private bool mBowIsDead;

        private readonly Texture2D mArrowTexture;


        private bool mArrowDestroy;

        private readonly float mLifeTime = 4;
        private float mWaitForArrow;
        private readonly int mAttack;


        public Archer(Texture2D texture,
            Texture2D arrowTexture,
            int posX,
            int posY,
            float speed,
            int attack,
            int health,
            int heat,
            Grid grid,
            GameState gameState,
            EFaction faction,
            Routine routine = Routine.Nothing) :
            base(texture, posX, posY, speed, attack, health, heat, grid, gameState, faction, routine)
        {
            mArrowRange = 5;
            //mProjectile = new Projectile(new Vector2(posX, posY), texture, grid, mArrowRange);
            mArrowTexture = arrowTexture;
            mAttack = attack;
            //mAttackSave = attack;

        }

        public int GetArrowDamage()
        {
            if (Bow == null)
            {
                return 0;
            }
            if(Bow.ReturnBowType() == Bow.BowType.Holz)
            {
                return 20;
            }
            return 30;
        }

        public Archer(Texture2D texture, Texture2D arrowTexture, Grid grid, GameState gameState, ArcherModel model) :
            base(texture, grid, gameState, model)
        {
            mArrowRange = model.mArrowRange;
            mArrowTexture = arrowTexture;
            if (model.mHasBow)
            {
                Bow.BowType type = model.mBowMaterial switch
                {
                    "Metall" => Bow.BowType.Metall,
                    _ => Bow.BowType.Holz
                };
                Debug.Assert(model.mBowUses != null, "model.mAxeUses != null");
                Bow = new Bow(type, false, 5, (int)model.mBowUses);
                Tool = type switch
                {
                    Bow.BowType.Holz => new AbstractEquipment(gameState.mWeapontexture,
                        AbstractEquipment.ItemType.Holzbogen),
                    _ => new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallbogen)
                };
            }
        }
        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            ArcherModel model = new ArcherModel()
            {
                mArrowRange = mArrowRange,
                Attack = GetAttack(),
                Faction = Faction.ToString(),
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y,
                Health = Health,
                Heat = Heat,
                IsDead = IsDead,
                Saturation = Saturation,
                mHasBow = Bow != null,
                mBowMaterial = Bow?.ReturnTypeAsString(),
                mBowUses = Bow?.NumberOfArrows,
                HasArmor = Armor != null
            };
            return model;
        }
        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            base.Update(gameTime, grid, gameState, soundManager);
            mWaitForArrow -= (float)gameTime.ElapsedGameTime.TotalSeconds; 
            if (!IsMoving() && mSelected)
            {
                if (gameState.mInputHandler.MouseClickRight)
                {
                    var gridDestination =
                        PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gridDes =
                        mGrid.PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gameObject = mGrid.GetAbstractGameObjectAt(gridDes);
                    if (gameObject is EmptyObject)
                    {
                        if (!gameState.mInputHandler.SkillKeyDown)
                        {
                            SetGridTarget(gridDestination.X, gridDestination.Y);
                        }
                    }
                    if (IsShootable(gameObject) || gameState.mInputHandler.SkillKeyDown)
                    {
                        InputHandler.InputList.BlockFurtherInput();
                        // shoot Arrow
                        var relativeGridDestination =
                            new GridPosition(gridDestination.X - GetGridPos().X, gridDestination.Y - GetGridPos().Y);
                        var range = Math.Sqrt(relativeGridDestination.X * relativeGridDestination.X +
                                              relativeGridDestination.Y * relativeGridDestination.Y);

                        if (mWaitForArrow < 0) // ready for shooting
                        {
                            if (range <= mArrowRange)
                            {

                                AddArrow(gameState.mInputHandler.MousePosition);
                                gameState.mManagers.mSoundManager.Bogen_schießen_sound();
                                if (Bow != null)
                                {
                                    Bow.NumberOfArrowsShot += 1;
                                }

                                mWaitForArrow = mLifeTime / 2;

                            }
                        }
                    }
                }
            }

            if (Bow != null)
            {

                Bow.Update(gameTime, mGrid,  gameState);
                GetEquipment(Bow);

                if (!mBowIsDead)
                {
                    //more damage with  holz or metall bow
                    if (mBowType == "Holz")

                    {
                        Attack = mAttack * 2;
                    }
                    else if (mBowType == "Metall")
                    {
                        Attack = mAttack * 3;
                    }
                }
                else
                {
                    Bow = null;
                    Tool = null;
                    Attack = mAttack;
                }
               
            }
            
                
            if (mArrowList.Any())
            {
                Arrow[] arrowList = mArrowList.ToArray();
                foreach (var arrow in arrowList)
                {
                    arrow.Update(gameTime, mGrid, gameState);
                }
                for(int i = 0; i< mArrowList.Count; i++)
                {
                    if (mArrowList[i].mDestroy)// if arrow destroyd remove
                    {
                        mArrowList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private bool IsShootable(AbstractGameObject gameObject)
        {
            return gameObject is Human || gameObject is Wolf;

        }
        
        public override bool MissingEquipment()
        {
            return Bow == null;
        }

        private float GetFiringAngle(Vector2 startPosition, Vector2 mousePosition)// calculate the firing angle
        {
            Vector2 direction= mousePosition - startPosition;
            float rad = (float)Math.Atan(direction.Y/direction.X);
            // have to take care with negative value in arctan ex: arctan(-4/3) = arctan(3/-4)
            // so we have to add 0,180 or 360
            float angle = MathHelper.ToDegrees(rad);
            if (direction.X <0 && direction.Y >= 0)  
            {
                
                return angle + 180;
            }
            else if(direction.X >= 0 && direction.Y < 0)
            {
                
                return angle + 360;
                
            }
            else if((direction.X < 0 && direction.Y < 0))
            {

                
                return angle + 180;
            }
            else
            {
                
                return angle;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
            if (mArrowList.Any())
            {
                foreach (var arrow in mArrowList.ToArray())
                {
                    arrow.Draw(spriteBatch, camera);
                }
            }
        }

        // get hypothenuse of rechtangle
        private Vector2 GetMovmentVector(Vector2 startPosition, Vector2 mousePosition )
        {
            Vector2 movment = mousePosition - startPosition;
            movment.Normalize();
            return movment;
            
        }

        private void AddArrow(Vector2 targetPosition)
        {
            var tar = mGrid.PositionToCoordinatesRelativeToCamera(targetPosition);
            targetPosition = new Vector2(tar.X , tar.Y);
            Vector2 position = new Vector2((float)mGridPos.X * mFieldSize + mFieldSize/2f, (float)mGridPos.Y * mFieldSize + mFieldSize/2f);// arrow is in middle of archer
            if (Vector2.Equals(targetPosition, position))
            {
                return;
            }
            Vector2 movement = GetMovmentVector(position, targetPosition);
            mArrowDestroy = false;
            Arrow arrow = new Arrow(position, movement, GetFiringAngle(position, targetPosition), mLifeTime, Attack, Faction, mArrowTexture,mArrowDestroy);
            
            mArrowList.Add(arrow);
        }
        public void GetEquipment(Bow bow)
        {
            Bow = bow;
            mBowType = bow.ReturnTypeAsString();
            mBowIsDead = bow.IsDead;
        }

        public void ShootAt(AbstractGameObject target, GameState gameState)
        {
            if (mWaitForArrow < 0) // ready for shooting
            {
                AddArrow(new Vector2(target.GetPos().X + mFieldSize /2, target.GetPos().Y + mFieldSize/2));
                gameState.mManagers.mSoundManager.Bogen_schießen_sound();
                if (Bow != null)
                {
                    Bow.NumberOfArrowsShot += 1;
                }
                mWaitForArrow = mLifeTime / 2;
            }
        }

        private bool IsInRange(AbstractGameObject target)
        {
            return (DistanceTo(target)/mFieldSize <= mArrowRange);
        }

        public bool CanShoot(AbstractGameObject target)
        {
            if (!IsInRange(target))
            {
                return false;
            }

            var startPos = new Vector2(mPosition.X - (float) mFieldSize / 2,
                mPosition.Y - (float) mFieldSize / 2);
            var endPos = new Vector2(target.GetPos().X - mFieldSize / 2, target.GetPos().Y - mFieldSize / 2);
            var startGridPos = mGridPos;
            var endGridPos = target.GetGridPos();
            startGridPos.X = (startGridPos.X < endGridPos.X) ? startGridPos.X : endGridPos.X;
            startGridPos.Y = (startGridPos.Y < endGridPos.Y) ? startGridPos.Y : endGridPos.Y;
            endGridPos.X = (startGridPos.X > endGridPos.X) ? startGridPos.X : endGridPos.X;
            startGridPos.Y = (startGridPos.Y > endGridPos.Y) ? startGridPos.Y : endGridPos.Y;
            for (int i = startGridPos.X + 1; i < endGridPos.X; i++)
            {
                for (int j = startGridPos.Y + 1; j < endGridPos.Y; j++)
                {
                    var objectAtPos = mGrid.GetAbstractGameObjectAt(new Point(i, j));
                    if (!(objectAtPos is ResourceObject || objectAtPos is AbstractMoveableObject))
                    {
                        continue;
                    }

                    if (ColLineRect(startPos.X,
                        startPos.Y,
                        endPos.X,
                        endPos.Y,
                        i * mFieldSize,
                        j * mFieldSize,
                        mFieldSize,
                        mFieldSize))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ColLineRect(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh)
        {
            var left = ColLineLine(x1, y1, x2, y2, rx, ry, rx, ry + rh);
            var right = ColLineLine(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh);
            var top = ColLineLine(x1, y1, x2, y2, rx, ry, rx + rw, ry);
            var bottom = ColLineLine(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh);
            
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }

        private bool ColLineLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }

        internal override void TryToCraftToolEnemy()
        {
            if (Faction != EFaction.Enemy)
            {
                return;
            }
            
                if (mGameState.AbleToBuildMetallbogen(true))
                {
                    GetEquipment(mGameState.GamePlayObjectFactory.CreateBow(Bow.BowType.Metall));
                    mGameState.mEnemyResources.Decrease(ResourceType.Metall,
                        GameState.QuantityOfMetallNeededForMetallbogen);
                    return;
                }

                if (mGameState.AbleToBuildHolzbogen(true))
                {
                    GetEquipment(mGameState.GamePlayObjectFactory.CreateBow(Bow.BowType.Holz));
                    mGameState.mEnemyResources.Decrease(ResourceType.Wood,
                        GameState.QuantityOfWoodNeededForHolzbogen);
                }
        }
    }
}
