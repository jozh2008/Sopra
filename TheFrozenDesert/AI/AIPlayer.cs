using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Priority_Queue;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;

namespace TheFrozenDesert.AI
{
    enum Goals
    {
        CollectResources,
        CollectFood,
        FightPlayer,
        WarmUnits,
        CraftTools,
        Reinforcements,
        BuildCampfire,
        RepositionUnits
    }

    internal sealed class AiPlayer
    {
        private readonly GamePlayObjectFactory mGamePlayObjectFactory;

        private readonly List<Human> mUnits;

        private Goals mActiveGoal;

        private DangerLevel mDangerLevel;
        private double mTimer;

        // timing
        private bool mIsActive;
        private double mTimeSinceLastRecruitment;
        private double mTimeSinceLastReposition;
        private double mWaitTime;
        private readonly Resources mResources;
        private readonly Random mRandom;
        private readonly Camera mCamera;
        private readonly Grid mGrid;
        private bool mIsDisabled;
        private double mAge;

        public AiPlayer(GamePlayObjectFactory gamePlayObjectFactory,
            int waitTime,
            Resources resources,
            GameState gameState, bool isTechDemo)
        {
            mGamePlayObjectFactory = gamePlayObjectFactory;
            mUnits = new List<Human>();
            mWaitTime = waitTime;
            mResources = resources;
            mActiveGoal = Goals.CollectResources;
            mCamera = gameState.GetCamera();
            mRandom = new Random(gameState.GetGameStateId());
            mGrid = gameState.mGrid;
            mIsDisabled = isTechDemo;
            mAge = 0;
        }

        public void AddHuman(Human human)
        {
            mUnits.Add(human);
        }

        public void SetSledge(Sledge sledge)
        {
            mDangerLevel = new DangerLevel(sledge);
        }

        public void Update(GameTime gameTime, Camera camera, GameState gameState)
        {
            if (mIsDisabled)
            {
                return;
            }
            HandleDangerLevel(gameTime, camera);
            mTimeSinceLastRecruitment += gameTime.ElapsedGameTime.TotalSeconds;
            mTimeSinceLastReposition += gameTime.ElapsedGameTime.TotalSeconds;
            mAge += gameTime.ElapsedGameTime.TotalSeconds;
            mUnits.RemoveAll(e => !e.IsAlive());
            UpdateActive(gameTime);
            if (mIsActive)
            {
                UpdateGoal(gameState);
                UpdateRoutines();
                mIsActive = false;
            }
        }

        public void Enable()
        {
            mIsDisabled = false;
            mAge = 180;
            mTimeSinceLastReposition = -9999999;
        }

        private void UpdateActive(GameTime gameTime)
        {
            mWaitTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (mWaitTime <= 0)
            {
                mIsActive = true;
                return;
            }
            mIsActive = false;
        }

        private struct Priorities
        {
            public int mCollectResources;
            public int mWarmHumans;
            public int mFightPlayer;
            public int mReinforcements;
            public int mBuildCampfire;
            public int mCraftTools;
            public int mCollectFood;
            public int mRepositionUnit;
        }

        private void UpdateGoal(GameState gameState)
        {
            SimplePriorityQueue<Goals, int> priorityQueue = new SimplePriorityQueue<Goals, int>();
            // checks the situation for the moment and switches the goal
            Priorities priorities = new Priorities();
            bool humanWithoutEquipment = false;
            foreach (var human in mUnits)
            {
                if (human.IsCold())
                {
                    priorities.mWarmHumans -= 250;
                    if (human.FindClosestCampfire() == null)
                    {
                        priorities.mBuildCampfire -= 300;
                        if (gameState.mEnemyResources.ResourceDictionary[ResourceType.Wood] < Human.CostCampfire)
                        {
                            priorities.mCollectResources -= 350;
                        }
                    }
                    else if (human.DistanceTo(human.FindClosestCampfire()) > 10)
                    {
                        priorities.mBuildCampfire -= 50;
                        if (gameState.mEnemyResources.ResourceDictionary[ResourceType.Wood] < Human.CostCampfire)
                        {
                            priorities.mCollectResources -= 150;
                        }
                    }
                }

                if (human.IsHungry())
                {
                    priorities.mCollectFood -= 150;
                }
                if (human is Fighter fighter)
                {
                    priorities.mFightPlayer -= human.Health;
                    priorities.mFightPlayer -= human.Health/(100/fighter.BaseAttackPower());
                    if (fighter.Sword == null)
                    {
                        humanWithoutEquipment = true;
                    }
                }
                if (human is Archer archer)
                {
                    if (archer.GetArrowDamage() > 0)
                    {
                        priorities.mFightPlayer -= human.Health;
                        priorities.mFightPlayer -= human.Health / (100 / archer.GetArrowDamage());
                    }
                    else
                    {
                        humanWithoutEquipment = true;
                    }
                }
                if (mAge < 180)
                {
                    priorities.mFightPlayer += 100000;
                }
                if (OutsidePlayerView(human) && mTimeSinceLastReposition > 30)
                {
                    Debug.WriteLine(human.GetGridPos());
                    priorities.mRepositionUnit -= 250;
                }
            }

            priorities.mFightPlayer += gameState.mPlayerAliveHumans.Count * 50;
            if (gameState.mPlayerAliveHumans.Count > mUnits.Count)
            {
                priorities.mReinforcements -= (int) Math.Floor(10 * mTimeSinceLastRecruitment);
            }

            if (mAge > 400)
            {
                priorities.mReinforcements -= (int)Math.Floor(mTimeSinceLastRecruitment);
            }

            if (humanWithoutEquipment)
            {
                if (mResources.ResourceDictionary[ResourceType.Metall] >= 10)
                {
                    priorities.mCraftTools -= 200 * mRandom.Next(1, 8);
                }
                else
                {
                    priorities.mCollectResources -= 200 * mRandom.Next(1, 5);
                }
            }
            else
            {
                priorities.mCollectResources -= 100;
            }

            priorityQueue.Enqueue(Goals.BuildCampfire, priorities.mBuildCampfire);
            priorityQueue.Enqueue(Goals.WarmUnits, priorities.mWarmHumans);
            priorityQueue.Enqueue(Goals.CollectFood, priorities.mCollectFood);
            priorityQueue.Enqueue(Goals.CollectResources, priorities.mCollectResources);
            priorityQueue.Enqueue(Goals.FightPlayer, priorities.mFightPlayer);
            priorityQueue.Enqueue(Goals.CraftTools, priorities.mCraftTools);
            priorityQueue.Enqueue(Goals.Reinforcements, priorities.mReinforcements);
            priorityQueue.Enqueue(Goals.RepositionUnits, priorities.mRepositionUnit);
            mActiveGoal = priorityQueue.First;
            
        }

        private bool OutsidePlayerView(AbstractGameObject human)
        {
            var screenSize = mCamera.GetScreenSize();
            return human.GetAbsolutePos().X < mCamera.PositionPixels.X ||
                   human.GetAbsolutePos().Y < mCamera.PositionPixels.Y ||
                   human.GetAbsolutePos().X > mCamera.PositionPixels.X + screenSize.X ||
                   human.GetAbsolutePos().Y > mCamera.PositionPixels.Y + screenSize.Y;
        }

        private void UpdateRoutines()
        {
            // checks for active goal and assigns routines to units
            Debug.WriteLine(mActiveGoal.ToString());
            switch (mActiveGoal)
            {
                case Goals.CollectResources:
                    foreach (var human in mUnits)
                    {
                        human.mRoutineHandler.SetRoutine(Routine.Gather);
                    }
                    mWaitTime = 15.0;
                    break;
                case Goals.FightPlayer:
                    foreach (var human in mUnits)
                    {
                        if (human.FindClosestOpponent() != null && human.DistanceTo(human.FindClosestOpponent()) < 640)
                        {
                            human.mRoutineHandler.SetRoutine(Routine.Attack);
                        }
                    }
                    mWaitTime = 15.0;
                    break;
                case Goals.WarmUnits:
                    foreach (var human in mUnits)
                    {
                        if (human.IsCold())
                        {
                            human.mRoutineHandler.SetRoutine(Routine.Warmup);
                        }
                    }
                    mWaitTime = 5.0;
                    break;
                case Goals.BuildCampfire:
                    int index = mRandom.Next(mUnits.Count);
                    if (mUnits.Count != 0)
                    {
                        mUnits[index].mRoutineHandler.SetRoutine(Routine.BuildCampfire);
                        mWaitTime = 1.0;
                    }
                    break;
                case Goals.Reinforcements:
                    Point pos = RandomGridPositionOutsidePlayerView(mCamera);
                    if (mGrid.GetAbstractGameObjectAtNewGridPosition(pos.ToVector2()) is EmptyObject emptyObject && !emptyObject.IsBlocked())
                    {
                        var type = mRandom.Next(3);
                        switch (type)
                        {
                            case 0:
                                mGamePlayObjectFactory.CreateGatherer(pos.X, pos.Y, EFaction.Enemy);
                                break;
                            case 1:
                                mGamePlayObjectFactory.CreateFighter(pos.X, pos.Y, EFaction.Enemy);
                                break;
                            case 2:
                                mGamePlayObjectFactory.CreateArcher(pos.X, pos.Y, EFaction.Enemy);
                                break;
                        }

                        mWaitTime = 1.0;
                        mTimeSinceLastRecruitment = 0.0;
                    }
                    break;
                case Goals.CraftTools:
                    foreach (var human in mUnits)
                    {
                        if (human.MissingEquipment())
                        {
                            human.TryToCraftToolEnemy();
                        }
                    }
                    mWaitTime = 1.0;
                    break;
                case Goals.RepositionUnits:
                    foreach (var human in mUnits)
                    {
                        if (OutsidePlayerView(human))
                        {
                            Point pos2 = RandomGridPositionOutsidePlayerView(mCamera);
                            human.SetGridTarget(pos2.X, pos2.Y);
                        }
                    }
                    mTimeSinceLastReposition = 0;
                    mWaitTime = 10;
                    break;
            }
        }

        /*
        private void Wait()
        {
            foreach (var human in mUnits)
            {
                human.mRoutineHandler.SetRoutine(Routine.Idle);
            }
        }
        */

        private void HandleDangerLevel(GameTime gameTime, Camera camera)
        {
            mDangerLevel.Update(gameTime);
            mTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimer > 1 && (float) Game1.mGlobalRandomNumberGenerator.Next(1, 1000) / 1000 < mDangerLevel.GetNormalizedDanger())
            {
                mTimer = 0;
                var randomPosition = RandomGridPositionOutsidePlayerView(camera);
                var emptyObject = (EmptyObject) mGrid.GetAbstractGameObjectAtGridPosition(new Vector2(randomPosition.X, randomPosition.Y));
                if (!emptyObject.IsBlocked())
                {
                    mGamePlayObjectFactory.CreateFighter(randomPosition.X, randomPosition.Y, EFaction.Enemy, Routine.Attack);
                }
            } else if (mTimer > 1)
            {
                mTimer = 0;
            }
        }

        private Point RandomGridPositionOutsidePlayerView(Camera camera)
        {
            var screenSize = camera.GetScreenSizeFields();
            var randRange = (screenSize.X + screenSize.Y + 6) * 2;
            var randPos = Game1.mGlobalRandomNumberGenerator.Next(0, randRange);
            Point randGridPos = new Point(0,0);
            if (randPos <= screenSize.X + 1)
            {
                randGridPos.X = randPos;
            } else if (randPos <= screenSize.X + screenSize.Y + 2)
            {
                randGridPos.X = screenSize.X + 1;
                randGridPos.Y = randPos - screenSize.X - 1;
            } else if (randPos <= 2 * screenSize.X + screenSize.Y + 3)
            {
                randGridPos.X = randPos - screenSize.X - screenSize.Y - 2;
                randGridPos.Y = screenSize.Y + 1;
            }
            else
            {
                randGridPos.Y = randPos - 2 * screenSize.Y - screenSize.Y - 3;
            }

            randGridPos.X += (int) camera.PositionFields.X - 1;
            randGridPos.Y += (int) camera.PositionFields.Y - 1;

            if (randGridPos.X < 0)
            {
                randGridPos.X += screenSize.X + 1;
            } else if (randGridPos.X > Game1.MapWidth)
            {
                randGridPos.X -= screenSize.X + 1;
            }

            if (randGridPos.Y < 0)
            {
                randGridPos.Y += screenSize.Y + 1;
            } else if (randGridPos.Y > Game1.MapHeight)
            {
                randGridPos.Y -= screenSize.Y + 1;
            }

            return randGridPos;
        }
    }
}
