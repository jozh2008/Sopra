using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.AI;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Gatherers : Human
    {
        private const int IncreasingValue = 2; // Value that a gather Earning increases every 5 time
        private readonly GamePlayObjectInfoBar mInfoBar;
        private readonly double mMiningSpeed;
        private int mEarningIncreaseRock; // Value is added to Earning increases if Gatherer mines often
        private int mEarningIncreaseTree; // Value is added to Earning increases if Gatherer mines often
        private AbstractGameObject mMiningTarget;
        private int mNumberOfTimeCutTree;
        private int mNumberOfTimeMineRock;
        private Axe mAxe;
        private bool mReady = true;
        private bool mGatherSnowTile;
        private readonly int mAttack; // how strong gatherer hits big tree or snowtile
        private readonly double mPauseTime;
        private double mTimeSinceInfoBarDecrease;
        //for leveling
        private double mTimeSinceLastMining;
        
        public Gatherers(Texture2D texture,
            int posX,
            int posY,
            float speed,
            int attack,
            int health,
            int heat,
            Grid grid,
            GameState gameState,
            double miningSpeed,
            double pauseTime,
            EFaction faction,
            Routine routine = Routine.Nothing) :
            base(texture, posX, posY, speed, attack, health, heat, grid, gameState, faction, routine)
        {
            mMiningSpeed = miningSpeed;
            mAttack = attack;
            mPauseTime = pauseTime;
            mInfoBar = new GamePlayObjectInfoBar(this, 32 - 5, Color.LightBlue);
        }

        public Gatherers(Texture2D texture, Grid grid, GameState gameState, GathererModel model) :
            base(texture, grid, gameState, model)
        {
            mMiningSpeed = model.mMiningSpeed;
            mEarningIncreaseRock = model.mEarningIncreaseRock;
            mEarningIncreaseTree = model.mEarningIncreaseTree;
            mNumberOfTimeCutTree = model.mNumberOfTimeCutTree;
            mNumberOfTimeMineRock = model.mNumberOfTimeMineRock;
            mTimeSinceLastMining = model.mTimeSinceLastMining;
            if (model.mHasAxe)
            {
                Axe.AxeType type = model.mAxeMaterial switch
                {
                    "Metall" => Axe.AxeType.Metall,
                    _ => Axe.AxeType.Holz
                };
                Debug.Assert(model.mAxeUses != null, "model.mAxeUses != null");
                mAxe = new Axe(type, false, 5, (int)model.mAxeUses);
                Tool = type switch
                {
                    Axe.AxeType.Holz => new AbstractEquipment(gameState.mWeapontexture,
                        AbstractEquipment.ItemType.Holzaxt),
                    _ => new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallaxt)
                };
            }
            mInfoBar = new GamePlayObjectInfoBar(this, 32 - 5, Color.LightBlue);
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            GathererModel model = new GathererModel
            {
                mEarningIncreaseRock = mEarningIncreaseRock,
                mEarningIncreaseTree = mEarningIncreaseTree,
                mMiningSpeed = mMiningSpeed,
                mNumberOfTimeCutTree = mNumberOfTimeCutTree,
                mNumberOfTimeMineRock = mNumberOfTimeMineRock,
                mTimeSinceLastMining = mTimeSinceLastMining,
                Attack = Attack,
                Faction = Faction.ToString(),
                GridCurrentPositionX = mGridPos.X,
                GridCurrentPositionY = mGridPos.Y,
                Health = Health,
                Heat = Heat,
                IsDead = IsDead,
                Saturation = Saturation,
                mHasAxe = mAxe != null,
                mAxeMaterial = mAxe?.ReturnTypeAsString(),
                mAxeUses = mAxe?.NumberOfUsesAxe,
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
            if (mMiningTarget != null && Faction == EFaction.Player &&!mGrid.IsGridCoordinateInScreen(mMiningTarget.GetGridPos().X, mMiningTarget.GetGridPos().Y))
            {
                mMiningTarget = null;
                mRoutineHandler.SetRoutine(Routine.Nothing);
                return;
            }

            if (mMiningTarget is ResourceObject || mMiningTarget is SnowTile)
            {
                Mining(mMiningTarget, gameState, gameTime, mGrid,mAxe, soundManager);
            } else if (mMiningTarget is Bridge)
            {
                RepairBridge((Bridge) mMiningTarget, gameTime, gameState);
            } else if (mMiningTarget is BigTree || mMiningTarget is BigTreePart)
            {
                MineBigTree(gameTime);
            }


            if (!IsMoving() && mSelected)
            {
                if (gameState.mInputHandler.MouseClickRight)
                {
                    var gridDestination =
                        PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gridDes =
                        mGrid.PositionToGridCoordinatesRelativeToCamera(gameState.mInputHandler.MousePosition);
                    var gameObject = mGrid.GetAbstractGameObjectAt(gridDes);

                    if (!(gameObject is ResourceObject || gameObject is SnowTile || gameObject is Bridge || gameObject is BigTree || gameObject is BigTreePart))
                    {
                        SetGridTarget(gridDestination.X, gridDestination.Y);
                    }
                    else
                    {
                        mReady = false; // gather is busy with mining the target
                        
                        SetDestinationObject(gameObject);

                        mMiningTarget = gameObject;
                    }
                }
                if (gameState.mInputHandler.MouseClickRight && gameState.mInputHandler.SkillKeyDown)

                {
                    AddCampfire(gameState);
                }
            }
            
        }

        public override bool MissingEquipment()
        {
            return mAxe == null;
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)

        {
            base.Draw(spriteBatch, camera);
            if (!mReady && !mGatherSnowTile)
            {
                mInfoBar.Draw(spriteBatch, camera);
            }
            
        }

        //MiningSpeed dependence of tool(Not finished yet)
        private double MiningSpeed(string nameOfTool)
        {
            if (nameOfTool == "Holz")
            {
                return mMiningSpeed / 2;
            }

            if (nameOfTool == "Metall")
            {
                return mMiningSpeed / 4;
            }

            return mMiningSpeed;
        }

        internal override void TryToCraftToolEnemy()
        {
            if (Faction != EFaction.Enemy)
            {
                return;
            }
            
                if (mGameState.AbleToBuildMetallaxt(true))
                {
                    GetEquipment(mGameState.GamePlayObjectFactory.CreateAxe(Axe.AxeType.Metall));
                    mGameState.mEnemyResources.Decrease(ResourceType.Metall,
                        GameState.QuantityOfMetallNeededForMetallaxt);
                    return;
                }

                if (mGameState.AbleToBuildHolzaxt(true))
                {
                    GetEquipment(mGameState.GamePlayObjectFactory.CreateAxe(Axe.AxeType.Holz));
                    mGameState.mEnemyResources.Decrease(ResourceType.Wood,
                        GameState.QuantityOfWoodNeededForHolzaxt);
                }
        }

        private int GetMyAttack()
        {
            if (mAxe != null)
            {
                var type = mAxe.ReturnTypeAsString();
                if (type == "Metall")
                {
                    return mAttack * 4;
                }
                if (type == "Holz")
                {
                    return mAttack * 2;
                }
            }

            return mAttack;
        }

        private void Mining(AbstractGameObject gameObject,
            GameState gameState,
            GameTime gameTime,
            Grid grid2,
            Axe axe, 
            SoundManager soundManager)
        {
            if (!(gameObject is ResourceObject || gameObject is SnowTile))
            {
                mInfoBar.SetPercent(100);
                mReady = true;
                return;
            }

            if (gameObject is ResourceObject resourceObject)
            {
                //prototype 
                var nameOfTool = "";
                if (axe != null)
                {
                    axe.Update(gameTime, mGrid, gameState);
                    if (!axe.IsDead)
                    {

                        nameOfTool = axe.ReturnTypeAsString();
                    }
                    else
                    {
                        mAxe = null;
                        Tool = null;
                    }
                }


                // time management
                if (mReady)
                {
                    mTimeSinceLastMining = 0;
                }
                else // gather is busy 
                {
                    //Debug.WriteLine(MiningSpeed(nameOfTool));
                    if (IsNeighbor(mGridPos,
                            new GridPosition(resourceObject.GetGridPos().X, resourceObject.GetGridPos().Y)))
                    {
                        mTimeSinceInfoBarDecrease += gameTime.ElapsedGameTime.TotalSeconds;
                        mTimeSinceLastMining += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (mTimeSinceLastMining > MiningSpeed(nameOfTool) )
                    {
                        // Mining is finished
                        mReady = true;
                        mTimeSinceLastMining = 0;
                        if (axe != null && !axe.IsDead)
                        {
                            axe.NumberOfUsesAxe += 1;
                        }

                        mInfoBar.SetPercent(100);
                        // remove resource Object
                        grid2.RemoveObjectFromGridPosition(resourceObject.GetGridPos());
                        PlaySound(soundManager, resourceObject);
                        if (!gameState.GetGame().AchievementsModel.mWood &&
                            resourceObject.ReturnResourceType == ResourceType.Wood)
                        {
                            gameState.GetGame().AchievementsModel.mWood = true;
                            gameState.GetGame().GetSafeLoadManager()
                                .SaveAchievements(gameState.GetGame().AchievementsModel);
                        }

                        if (!gameState.GetGame().AchievementsModel.mMetal &&
                            resourceObject.ReturnResourceType == ResourceType.Metall)
                        {
                            gameState.GetGame().AchievementsModel.mMetal = true;
                            gameState.GetGame().GetSafeLoadManager()
                                .SaveAchievements(gameState.GetGame().AchievementsModel);
                        }

                        IncreaseRessources(resourceObject);
                        mRoutineHandler.SetRoutine(Routine.Gather);
                    }

                    // handle decreasing of the info-bar
                    if (mTimeSinceInfoBarDecrease > MiningSpeed(nameOfTool) / 4)
                    {
                        mInfoBar.Decrease(25);
                        mTimeSinceInfoBarDecrease = 0;
                    }
                }
            }
            else if (gameObject is SnowTile snowTile)
            {
                // logic for removing snowTile
                if (mReady)
                {
                    mTimeSinceLastMining = 0;
                }
                else // gather snowtile (gatherer is busy)
                {
                    mGatherSnowTile = true;
                    if (IsNeighbor(mGridPos,
                            new GridPosition(snowTile.GetGridPos().X, snowTile.GetGridPos().Y)))
                    {
                        mTimeSinceLastMining += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (mTimeSinceLastMining >= mPauseTime)
                    { 
                        mTimeSinceLastMining = 0;
                        mInfoBar.SetPercent(snowTile.Damage(GetMyAttack()));
                        if (snowTile.IsDestroyed())
                        {
                            // Mining is finished
                            mReady = true;
                            mGatherSnowTile = false;
                        }
                    }

                }
            }
        }

        private void PlaySound(SoundManager soundManager, ResourceObject resourceObject)
        {
            if (Faction == EFaction.Player)
            {
                if (resourceObject.mResourceObjectType == ResourceObject.ResourceObjectType.Rock)
                {
                    soundManager.Fels_abbauen_sound();
                }
                else if (resourceObject.mResourceObjectType == ResourceObject.ResourceObjectType.Tree)
                {
                    soundManager.Baum_abbauen_sound();
                }
            }

        }

        private void IncreaseRessources(ResourceObject resourceObject)
        {
            var resourceObjectType = resourceObject.ReturnResourceObjectTypeAsString();
            //Debug.WriteLine(resourceObject.Earning);
            switch (Faction)
            {
                case EFaction.Player:
                    mGameState.mResources.Increase(resourceObject.ReturnResourceType,
                        resourceObject.Earning + ReturnIncreaseValue(resourceObjectType));
                    break;
                case EFaction.Enemy:
                    mGameState.mEnemyResources.Increase(resourceObject.ReturnResourceType,
                        resourceObject.Earning + ReturnIncreaseValue(resourceObjectType));
                    break;
            }
        }

        private int ReturnIncreaseValue(string resourceObjectType)
        {
           
           
            if (resourceObjectType == "Rock")
            {
                 mNumberOfTimeMineRock += 1;
                if (mNumberOfTimeMineRock >= 5)
                {
                    mEarningIncreaseRock += IncreasingValue;
                    mNumberOfTimeCutTree = 0;
                }

                return mEarningIncreaseRock;
            }

            if (resourceObjectType == "Tree")
            {
                 mNumberOfTimeCutTree += 1;
                if (mNumberOfTimeCutTree >= 5)
                {
                    mEarningIncreaseTree += IncreasingValue;
                    mNumberOfTimeCutTree = 0;
                }

                return mEarningIncreaseTree;
            }

            return 0;
        }

        public void SetMiningTarget(ResourceObject resource)
        {
            if (resource != null)
            {
                mReady = false;
            }
            else
            {
                Stop();
            }
            mMiningTarget = resource;
        }

        private void Stop()
        {
                mReady = true;
                if (mInfoBar != null)
                {
                    mInfoBar.SetPercent(100);
                    mTimeSinceLastMining = 0;
                }
            
        }

        // Mines the closest resource object, if the gatherer isn't already mining.
        public void MineClosestResource()
        {
            if (!mReady)
            {
                return;
            }
            var closestResource = FindClosestResource();
            if (closestResource != null)
            {
                SetDestinationObject(closestResource);
                SetMiningTarget(closestResource);
            }
        }

        private void RepairBridge(Bridge bridge, GameTime gameTime, GameState gameState)
        {
            SetDestinationObject(bridge);
            if (IsNeighbor(mGridPos, new GridPosition(bridge.GetGridPos().X, bridge.GetGridPos().Y)) && !bridge.IsRepaired())
            {
                gameState.mManagers.mSoundManager.Build_sound();
                if (gameState.mResources.Get(ResourceType.Wood) > CostBridge)
                {
                    mInfoBar.SetPercent((int) (bridge.Repair(gameTime) * 100));
                }
                // TODO Add message that there is not enogh resources

                if (bridge.IsRepaired())
                {
                    gameState.mResources.Decrease(ResourceType.Wood, CostBridge);
                }
            }
        }

        private void MineBigTree(GameTime gameTime)
        {
            if (mMiningTarget is BigTree bigTree)
            {
                SetDestinationObject(bigTree);
                if (IsNeighbor(mGridPos, new GridPosition(bigTree.GetGridPos().X, bigTree.GetGridPos().Y)))
                {
                    mTimeSinceLastMining += gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (mTimeSinceLastMining >= mPauseTime)
                {
                    mTimeSinceLastMining = 0;
                    if (bigTree.Damage(GetMyAttack()))
                    {
                        Stop();
                    }
                }
            }
            else if (mMiningTarget is BigTreePart bigTreePart)
            {
                SetDestinationObject(bigTreePart);
                if (IsNeighbor(mGridPos, new GridPosition(bigTreePart.GetGridPos().X, bigTreePart.GetGridPos().Y)))
                {
                    mTimeSinceLastMining += gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (mTimeSinceLastMining >= mPauseTime)
                {
                    mTimeSinceLastMining = 0;
                    if (bigTreePart.Damage(GetMyAttack()))
                    {
                        Stop();
                    }
                }
                
            }
            
        }

        private void AddCampfire(GameState gameState)
        {
            var targetPosition = gameState.mInputHandler.MousePosition;
            var tar = mGrid.PositionToGridCoordinatesRelativeToCamera(targetPosition); // grid positions as point
            var gridDestinationCampfire = PositionToGridCoordinatesRelativeToCamera(targetPosition);
            var gameObject = mGrid.GetAbstractGameObjectAt(tar);
            if (IsNeighbor(gridDestinationCampfire, mGridPos) && gameState.mResources.Get(ResourceType.Wood) >= CostCampfire && gameObject is EmptyObject)
            {
                mGridPositionCampfire = tar;

                mGameState.mManagers.mSoundManager.Lager_feuer_füllen_sound();
                mGameState.GamePlayObjectFactory.CreateCampfire(mGridPositionCampfire.X, mGridPositionCampfire.Y, mRadiusCampfire);
                gameState.mResources.Decrease(ResourceType.Wood, CostCampfire);
            }
        }
        public void GetEquipment(Axe axe)
        {
            mAxe = axe;
        }

    }
}