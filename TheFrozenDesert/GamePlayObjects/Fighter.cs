using System.Diagnostics;
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
    public sealed class Fighter : Human
    {
        private readonly int mTimeWaitForNextPunch = 1;


        private GridPosition mGridDestination;

        private bool mIsAlive = true; // opponent is alive

        private bool mIsReadyForNextPunch;

        private AbstractGameObject mTargetObject;

        private double mTimeSinceLastPunch;
        public Sword Sword { get; private set; }

        public Fighter(Texture2D texture,
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
            Health = health;
            Attack = attack;
        }

        public Fighter(Texture2D texture, Grid grid, GameState gameState, FighterModel model, Routine routine = Routine.Nothing) : base(texture,
            grid,
            gameState,
            model,
            routine)
        {
            Health = model.Health;
            Attack = model.Attack;
            if (model.mHasSword)
            {
                Sword.SwordType type = model.mSwordMaterial switch
                {
                    "Metall" => Sword.SwordType.Metall,
                    _ => Sword.SwordType.Holz
                };
                Debug.Assert(model.mSwordUses != null, "model.mAxeUses != null");
                Sword = new Sword(type, false, 5, (int)model.mSwordUses);
                Tool = type switch
                {
                    Sword.SwordType.Holz => new AbstractEquipment(gameState.mWeapontexture,
                        AbstractEquipment.ItemType.Holzschwert),
                    _ => new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallschwert)
                };
            }
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            FighterModel model = new FighterModel()
            {
                Attack = Attack,
                Faction = Faction.ToString(),
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y,
                Health = Health,
                Heat = Heat,
                IsDead = IsDead,
                Saturation = Saturation,
                mHasSword = Sword != null,
                mSwordMaterial = Sword?.ReturnTypeAsString(),
                mSwordUses = Sword?.NumberOfUsesSword,
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
            if (!IsMoving() && mSelected)
            {
                
                if (gameState.mInputHandler.MouseClickRight && !gameState.mInputHandler.SkillKeyDown)
                {
                    var gridDestination =
                        PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gridDes =
                        mGrid.PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gameObject = mGrid.GetAbstractGameObjectAt(gridDes);
                   
                   

                    if (gameObject is EmptyObject)      
                    {
                        SetGridTarget(gridDestination.X, gridDestination.Y);
                    }
                    else if (gameObject is Human || gameObject is Wolf)
                    {
                        InputHandler.InputList.BlockFurtherInput();
                        if (gameObject is Human human)
                        {
                            if (human.Faction != EFaction.Player)
                            {
                                SetPreAttack(gridDestination);
                                //Debug.WriteLine(gridDestination.X+"reerter");
                                mIsAlive = true;
                                mTargetObject = gameObject;
                            }
                        }
                        else
                        {
                            SetPreAttack(gridDestination);
                            //Debug.WriteLine(gridDestination.X+"reerter");
                            mIsAlive = true;
                            mTargetObject = gameObject;
                        }

                    }
                }
                
            }

            if (mRoutineHandler.CurrentRoutine != Routine.Attack)
            {
                AttackEnemy(mTargetObject, gameTime);
            }
        }

        public void SetPreAttack(GridPosition pos)
        {
            mGridDestination = pos;
            var left = pos.X < mGridPos.X ? 1 : 0;
            var right = pos.X > mGridPos.X ? 1 : 0;
            var up = pos.Y < mGridPos.Y ? 1 : 0;
            var down = pos.Y > mGridPos.Y ? 1 : 0;
            mIsAlive = true;
            SetGridTarget(pos.X + (left - right), pos.Y + (up - down));
        }

        public override bool MissingEquipment()
        {
            return Sword == null;
        }

        private void Punch(AbstractGameObject gameObject,
            GameState gameState)
        {
            GridPosition positionObject = new GridPosition(gameObject.GetGridPos().X, gameObject.GetGridPos().Y);
            //only punch if opponent, and neigbour(not on same gridposition)
            if (gameObject is Human humanObject)
            {
                //Debug.WriteLine(humanObject.Faction);
                if (humanObject.Faction != Faction && !(mGridDestination.X == mGridPos.X && mGridDestination.Y == mGridPos.Y) && IsNeighbor(positionObject,mGridDestination))
                {
                    if (humanObject.IsAlive() && mIsAlive)
                    {
                        if (humanObject.Faction == EFaction.Enemy)
                        {
                            humanObject.mRoutineHandler.SetRoutine(Routine.Attack);
                        }
                        humanObject.DamageHuman(AttackPower(Sword, Attack), gameState);
                        if (!humanObject.IsAlive())
                        {
                            mTargetObject = null;
                        }
                    }
                }
            }
            else if (gameObject is Wolf wolfObject)
            {
                if (!(mGridDestination.X == mGridPos.X && mGridDestination.Y == mGridPos.Y) && IsNeighbor(positionObject,mGridDestination))
                {
                    if (mIsAlive)
                    {
                        wolfObject.Damage(AttackPower(Sword, Attack), Faction);
                        if (wolfObject.Health <= 0)
                        {
                            mIsAlive = false;
                        }
                    }
                }
            }
        }

        // check the power of the attack
        private int AttackPower(Sword sword,int attack)
        {
            if (sword is {IsDead: false})
            {
                if (sword.ReturnTypeAsString() == "Holz")
                {
                    return 2 * attack;
                }
                if(sword.ReturnTypeAsString()== "Metall")
                {
                    return 3 * attack;
                }
            }
            
            return attack;
        }

        public int BaseAttackPower()
        {
            if (Sword is {IsDead: false})
            {
                if (Sword.ReturnTypeAsString() == "Holz")
                {
                    return 2 * Attack;
                }
                if (Sword.ReturnTypeAsString() == "Metall")
                {
                    return 3 * Attack;
                }
            }
            
            return Attack;
        }

        public void AttackEnemy(AbstractGameObject gameObject,
            GameTime gameTime)
        {
            if (gameObject is null)
            {
                return;
            }

            if (IsNeighbor(mGridPos, mGridDestination) && Attack > 0) //check if neighbor, if so start with Punch

            {
                //counter, wait always 1 second for next punch
                if (mIsReadyForNextPunch)
                {
                    
                    if(Sword is {IsDead: false})
                    {
                        Sword.Update(gameTime, mGrid, mGameState);
                        Sword.NumberOfUsesSword += 1;
                    }
                    else if(Sword is {IsDead: true})
                    {
                        Sword = null;
                        Tool = null;
                    }
                    mTimeSinceLastPunch = 0;
                    Punch(gameObject, mGameState);
                    mIsReadyForNextPunch = false;
                }
                else
                {
                    mTimeSinceLastPunch += gameTime.ElapsedGameTime.TotalSeconds;
                    if (mTimeSinceLastPunch > mTimeWaitForNextPunch)
                    {
                        // waitet 1 second
                        mIsReadyForNextPunch = true;
                    }
                }
            }

        }
        public void GetEquipment(Sword sword)
        {
            Sword = sword;
        }

        internal override void TryToCraftToolEnemy()
        {
            if (Faction != EFaction.Enemy)
            {
                return;
            }
            if (mGameState.AbleToBuildMetallschwert(true))
            {
                GetEquipment(mGameState.GamePlayObjectFactory.CreateSword(Sword.SwordType.Metall));
                mGameState.mEnemyResources.Decrease(ResourceType.Metall,
                    GameState.QuantityOfMetallNeededForMetallschwert);
                return;
            }

            if (mGameState.AbleToBuildHolzschwert(true))
            {
                GetEquipment(mGameState.GamePlayObjectFactory.CreateSword(Sword.SwordType.Holz));
                mGameState.mEnemyResources.Decrease(ResourceType.Wood,
                    GameState.QuantityOfWoodNeededForHolzschwert);
            }
        }
    }
}
