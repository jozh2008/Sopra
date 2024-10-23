using System;
using Microsoft.Xna.Framework;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;

namespace TheFrozenDesert.AI
{
    public enum Routine
    {
        Nothing,
        Idle,
        Attack,
        Gather,
        Warmup,
        Flight,
        Neutral,
        EnterGenerator,
        MoveToGenerator,
        BuildCampfire,
        CollectFood
    }

    /*  The RoutineHandler will control Units, according to a specified routine.
        Routines:
            Nothing:    The unit will do nothing (default)
            Idle:       The unit will move randomly within a 3x3 grid.
            Attack:     The unit will attack the closest unit of an opposing faction.
                        The unit must be either a fighter or archer.
            Gather:     The unit will mine the closest resource object.
                        The unit must be a gatherer.
            Warmup:     The unit finds the next heat source and goes there.
            Flight:     The unit tries to keep a distance to enemy units and wolfs.
            Neutral:    The unit will move randomly within a 4X4 grid. After movement the unit will wait for 4 seconds.
            MoveToGenerator:    The unit will move towards the Generator.
            EnterGenerator:     The unit will attempt to seize the Generator and succeed after a while.
     */
    public sealed class RoutineHandler
    {
        private readonly Human mUnit;
        public Routine CurrentRoutine { get; private set; }
        private Point mIdleStartPosition;
        private DateTime mIdleTimer;
        private readonly Grid mGrid;

        private const int FlightDistance = 10 * 32;

        public RoutineHandler(Human unit, Grid grid, Routine startRoutine = Routine.Nothing)
        {
            mUnit = unit;
            SetRoutine(startRoutine);
            mGrid = grid;
        }

        public void Update(GameTime gameTime, GameState gameState)
        {
            switch (CurrentRoutine)
            {
                case Routine.Nothing:
                    break;
                case Routine.Idle:
                    IdleRoutine();
                    break;
                case Routine.Attack:
                    AttackRoutine(gameTime, gameState);
                    break;
                case Routine.Gather:
                    GatherRoutine();
                    break;
                case Routine.Warmup:
                    WarmupRoutine();
                    break;
                case Routine.Flight:
                    FlightRoutine();
                    break;
                case Routine.Neutral:
                    NeutralRoutine();
                    break;
                case Routine.MoveToGenerator:
                    MoveToGeneratorRoutine();
                    break;
                case Routine.EnterGenerator:
                    EnterGeneratorRoutine(gameTime);
                    break;
                case Routine.BuildCampfire:
                    BuildCampfireRoutine();
                    break;
                case Routine.CollectFood:
                    CollectFoodRoutine(gameTime, gameState);
                    break;

            }
        }
        private void BuildCampfireRoutine()
        {
            mUnit.BuildCampfire();
            SetRoutine(Routine.Idle);
        }

        public void SetRoutine(Routine routine)
        {
            switch (routine)
            {
                case Routine.Idle:
                    mIdleTimer = DateTime.UtcNow;
                    mIdleStartPosition = mUnit.GetGridPos();
                    break;
                case Routine.Attack:
                    if (!(mUnit is Archer || mUnit is Fighter))
                    {
                        return;
                    }
                    break;
                case Routine.Gather:
                    if (!(mUnit is Gatherers))
                    {
                        return;
                    }
                    break;
                case Routine.CollectFood:
                    if (!(mUnit is Archer || mUnit is Fighter))
                    {
                        return;
                    }
                    break;
                // RoutineNothing only needs to be called once to stop all actions of the unit.
                case Routine.Nothing:
                    RoutineNothing();
                    break;
                case Routine.Neutral:
                    mIdleTimer = DateTime.UtcNow;
                    mIdleStartPosition = mUnit.GetGridPos();
                    break;
            }

            CurrentRoutine = routine;
        }
        private void EnterGeneratorRoutine(GameTime gameTime)
        {
            mUnit.ClaimGenerator(gameTime);
        }

        private void MoveToGeneratorRoutine()
        {
            mUnit.SetDestinationObject(mGrid.GetGenerator());
            if (mUnit.IsNeighbor(mUnit.GetGridPosition(), mGrid.GetGenerator().GetGridPosition()))
            {
                SetRoutine(Routine.EnterGenerator);
            }
        }

        private void IdleRoutine()
        {
            var currentTime = DateTime.UtcNow;
            if (mUnit.IsMoving())
            {
                mIdleTimer = currentTime;
            }
            if ((currentTime - mIdleTimer).TotalMilliseconds > Game1.mGlobalRandomNumberGenerator.Next(1000, 10000))
            {
                var randX = Game1.mGlobalRandomNumberGenerator.Next(0, 3) - 1;
                var randY = Game1.mGlobalRandomNumberGenerator.Next(0, 3) - 1;
                mUnit.SetGridTarget(mIdleStartPosition.X + randX, mIdleStartPosition.Y + randY);
            }
        }

        private void NeutralRoutine()
        {
            var currentTime = DateTime.UtcNow;
            if (mUnit.IsMoving())
            {
                mIdleTimer = currentTime;
            }
            if ((currentTime - mIdleTimer).TotalSeconds > 4)
            {
                var randX = Game1.mGlobalRandomNumberGenerator.Next(0, 4) - 1;
                var randY = Game1.mGlobalRandomNumberGenerator.Next(0, 4) - 1;
                mUnit.SetGridTarget(mIdleStartPosition.X + randX, mIdleStartPosition.Y + randY);
            }
        }

        private void AttackRoutine(GameTime gameTime, GameState gameState)
        {
            var closestOpponent = mUnit.FindClosestOpponent();
            if (closestOpponent != null && mUnit is Fighter fighter)
            {
                fighter.SetPreAttack(closestOpponent.GetGridPosition());
                fighter.SetDestinationObject(closestOpponent);
                fighter.AttackEnemy(closestOpponent, gameTime);
            }

            if (closestOpponent != null && mUnit is Archer archer)
            {
                archer.SetDestinationObject(closestOpponent);
                if (archer.CanShoot(closestOpponent))
                {
                    archer.StopMovement();
                    archer.ShootAt(closestOpponent, gameState);
                }
            }
        }

        private void CollectFoodRoutine(GameTime gameTime, GameState gameState)
        {
            var closestWolf = mUnit.FindClosestWolf();
            if (closestWolf != null && mUnit is Fighter fighter)
            {
                fighter.SetPreAttack(closestWolf.GetGridPosition());
                fighter.SetDestinationObject(closestWolf);
                fighter.AttackEnemy(closestWolf, gameTime);
            }

            if (closestWolf != null && mUnit is Archer archer)
            {
                archer.SetDestinationObject(closestWolf);
                if (archer.CanShoot(closestWolf))
                {
                    archer.StopMovement();
                    archer.ShootAt(closestWolf, gameState);
                }
            }
        }

        private void GatherRoutine()
        {
            var gatherer = (Gatherers) mUnit;
            gatherer.MineClosestResource();
        }

        private void WarmupRoutine()
        {
            var closestCampfire = mUnit.FindClosestCampfire();
            if (closestCampfire == null)
            {
                return;
            }
            mUnit.SetDestinationObject(closestCampfire);
        }

        private void FlightRoutine()
        {
            if (mUnit.IsMoving())
            {
                return;
            }
            var closestOpponent = mUnit.FindClosestOpponent();
            var closestWolf = mUnit.FindClosestWolf();
            AbstractMoveableObject closestThreat;
            if (closestOpponent == null && closestWolf == null)
            {
                return;
            }
            else if (closestWolf == null || (closestOpponent != null && mUnit.DistanceTo(closestOpponent) <= mUnit.DistanceTo(closestWolf)))
            {
                closestThreat = closestOpponent;
            }
            else
            {
                closestThreat = closestWolf;
            }

            if (mUnit.DistanceTo(closestThreat) < FlightDistance)
            {
                var dX = closestThreat.GetGridPos().X > mUnit.GetGridPos().X ? -1 : 1;
                dX = closestThreat.GetGridPos().X == mUnit.GetGridPos().X ? 0 : dX;
                var dY = closestThreat.GetGridPos().Y > mUnit.GetGridPos().Y ? -1 : 1;
                dY = closestThreat.GetGridPos().Y == mUnit.GetGridPos().Y ? 0 : dY;

                var newX = mUnit.GetGridPos().X + dX;
                var newY = mUnit.GetGridPos().Y + dY;
                while (newX >= 0 && newY >= 0 && newX < Game1.MapWidth && newY < Game1.MapHeight && !(mGrid.GetAbstractGameObjectAtGridPosition(new Vector2(newX,
                    newY)) is EmptyObject))
                {
                    newX += dX;
                    newY += dY;
                }
                mUnit.SetGridTarget(mUnit.GetGridPos().X + dX, mUnit.GetGridPos().Y + dY);
            }
        }

        private void RoutineNothing()
        {
            mUnit.StopMovement();
            if (mUnit is Gatherers gatherer)
            {
                gatherer.SetMiningTarget(null);
            }
        }
    }
}
