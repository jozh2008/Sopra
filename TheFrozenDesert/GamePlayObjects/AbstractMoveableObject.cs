using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Pathfinding;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public abstract class AbstractMoveableObject : AbstractGameObject
    {
        private Vector2 mDestination;
        private AbstractGameObject mDestinationObject;
        private bool mDestinationChange;
        private Point mGridDest;
        private List<Point> mMovementPath;
        private Vector2 mMovementVector = Vector2.Zero;

        private bool mSelectionChange;
        private bool mLastSelection;

        protected AbstractMoveableObject(Texture2D texture,
            int gridPosX,
            int gridPosY,
            float movementSpeed,
            Grid grid) : base(texture, gridPosX, gridPosY, grid)
        {
            MovementSpeed = movementSpeed;
            mMovementVector.X = 0;
            mMovementVector.Y = 0;
            mGridDest.X = gridPosX;
            mGridDest.Y = gridPosY;
            mDestinationObject = null;
            mMovementPath = new List<Point>();
        }

        //Movement Speed in pixels/second
        public float MovementSpeed { get; set; }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            UpdatePosition(gameTime, grid, gameState);
            if (IsMoving())
            {
                soundManager.Laufen_sound();
            }

            mSelectionChange = mSelected == !mLastSelection;
            mLastSelection = mSelected;
        }

        private void UpdatePosition(GameTime gameTime, AbstractGameObject[,] grid, GameState gameState)
        {
            if (!IsMoving())
            {
                UpdateGridPosition(grid, gameState);
            }

            UpdateDestination();

            if (mDestinationChange && !IsMoving())
            {
                mDestinationChange = false;
                UpdateMovementPath(gameState.mPathFinder);
            }

            if (!IsMoving() && mMovementPath.Count > 0)
            {
                MovementPathToNextDestination(gameState);
            }

            UpdateCoordinates(gameTime);
        }

        private void UpdateDestination()
        {
            if (mDestinationObject != null)
            {
                if (mDestinationObject.GetGridPos().Equals(mGridDest))
                {
                    SetGridTarget(mDestinationObject.GetGridPos().X, mDestinationObject.GetGridPos().Y, false);
                }
            }
        }

        protected virtual void UpdateMovementPath(PathFinder pathFinder)
        {
            mMovementPath = pathFinder.FindShortestGridPath(new Point(mGridPos.X, mGridPos.Y),
                new Point(mGridDest.X, mGridDest.Y),
                mGrid, (mDestinationObject != null));
            if (mDestinationObject != null && mMovementPath.Count >= 1)
            {
                mMovementPath.RemoveAt(mMovementPath.Count - 1);
            }
        }

        private void UpdateGridPosition(AbstractGameObject[,] grid, GameState gameState)
        {
            //Switch grid position if we are closer to the square destination than origin
            var oldPosition = new Point(mGridPos.X, mGridPos.Y);
            var positionChanged = false;
            if (gameState.mGrid.IsInGrid(mGridDest))
            {
                if (PositionToGridCoordinates(mPosition).X != mGridPos.X ||
                    PositionToGridCoordinates(mPosition).Y != mGridPos.Y)
                {
                    grid[mGridPos.X, mGridPos.Y] =
                        new EmptyObject(gameState.mMissingTexture, mGridPos.X, mGridPos.Y, mGrid);
                    mGridPos.X = PositionToGridCoordinates(mPosition).X;
                    mGridPos.Y = PositionToGridCoordinates(mPosition).Y;
                    grid[mGridPos.X, mGridPos.Y] = this;
                    positionChanged = true;
                }

                if (positionChanged)
                {
                    PostSwitchGridPosition(oldPosition);
                }
            }
        }

        private void MovementPathToNextDestination(GameState gameState)
        {
            gameState.mPathFinder.PathUnblockNodeAt(mGridPos.X, mGridPos.Y);
            var destination = mMovementPath[0];
            if (gameState.mPathFinder.NodeIsBlockedAt(destination.X, destination.Y, mGrid))
            {
                UpdateMovementPath(gameState.mPathFinder);
                // check if a new path could be found
                if (mMovementPath.Count > 0)
                {
                    destination = mMovementPath[0];
                }
            }
            // in case the path had to be recomputed, check if new path could be found
            if (mMovementPath.Count > 0)
            {
                mMovementPath.RemoveAt(0);
                gameState.mPathFinder.PathBlockNodeAt(destination.X, destination.Y);
                var pointPos = new Vector2(mFieldSize * destination.X, mFieldSize * destination.Y);
                SetPositionTarget(pointPos);
            }
        }

        protected virtual void PostSwitchGridPosition(Point previousPosition)
        {
        }

        private void UpdateCoordinates(GameTime gameTime)
        {
            if (IsMoving())
            {
                var newPosition = mPosition + Vector2.Multiply(mMovementVector,
                    (float) gameTime.ElapsedGameTime.TotalSeconds * MovementSpeed);
                if (Vector2.Distance(newPosition, mDestination) < Vector2.Distance(mPosition, mDestination))
                {
                    mPosition = newPosition;
                }
                else
                {
                    mPosition = mDestination;
                    mDestination = Vector2.Zero;
                    mMovementVector = Vector2.Zero;
                }
            }
        }

        // returns true when the object is newly selected
        protected bool JustSelected()
        {
            if (mSelected && mSelectionChange)
            {
                return true;
            }
            return false;
        }
        
        public bool IsMoving()
        {
            return mMovementVector.X != 0 || mMovementVector.Y != 0;
        }

        private void SetPositionTarget(Vector2 destination)
        {
            mDestination = destination;
            mMovementVector = mDestination - mPosition;
            if (IsMoving())
            {
                mMovementVector.Normalize();
            }
            //this.PostSetPositionTarget(destination);
        }

        public void SetGridTarget(int gridX, int gridY, bool resetDestinationObject = true)
        {
            if (resetDestinationObject)
            {
                mDestinationObject = null;
            }
            mGridDest.X = gridX;
            mGridDest.Y = gridY;
            mDestinationChange = true;
        }

        public void SetDestinationObject(AbstractGameObject gameObject)
        {
            if (gameObject != mDestinationObject)
            {
                mDestinationObject = gameObject;
                SetGridTarget(mDestinationObject.GetGridPos().X, mDestinationObject.GetGridPos().Y, false);
            }

        }

        public void StopMovement()
        {
            mMovementPath = new List<Point>();
            mDestinationObject = null;
            mGridDest = new Point(mGridPos.X, mGridPos.Y);
        }

        private GridPosition PositionToGridCoordinates(Vector2 position)
        {
            var pos = mGrid.PositionToGridCoordinates(position);
            return new GridPosition(pos.X, pos.Y);
        }

        protected GridPosition PositionToGridCoordinatesRelativeToCamera(Vector2 position)
        {
            var pos = mGrid.PositionToGridCoordinatesRelativeToCamera(position);
            return new GridPosition(pos.X, pos.Y);
        }
    }
}