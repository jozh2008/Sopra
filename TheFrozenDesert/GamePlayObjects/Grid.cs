using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public class Grid
    {
        private const int FieldSize = 32;

        private readonly Camera mCamera;
        private readonly GameState mGameState;

        //A grid of visible Spaces during gameplay
        private AbstractGameObject[,] mGrid;
        private AbstractGameObject[,] mNewGrid;
        private List<AbstractGameObject> mNonInteractableGameObjects;
        public List<Human> Humans { get; }
        public List<ResourceObject> ResourceObjects { get; }
        public List<Campfire> Campfires { get; }
        public List<Wolf> Wolfs { get; }
        private List<Key> Keys { get; }

        private Point mOrigin;
        private Point mSize;
        private int mSizeX;
        private int mSizeY;

        public Grid(GameState gameState, Camera camera)
        {
            mGameState = gameState;
            mCamera = camera;

            mNonInteractableGameObjects = new List<AbstractGameObject>();

            Humans = new List<Human>();
            ResourceObjects = new List<ResourceObject>();
            Campfires = new List<Campfire>();
            Wolfs = new List<Wolf>();
            Keys = new List<Key>();
        }

        public Point Size => mSize;

        public void InitGrid(int originX, int originY, int sizeX, int sizeY, Grid grid)
        {
            mSize = new Point(sizeX, sizeY);
            mSizeX = sizeX;
            mSizeY = sizeY;
            mGrid = new AbstractGameObject[sizeX,sizeY];
            for (var i = 0; i < sizeX; i++)
            {
                for (var j = 0; j < sizeY; j++)
                {
                    mGrid[i, j] = new EmptyObject(mGameState.mMissingTexture, i, j, grid);
                }
            }

            mNewGrid = new AbstractGameObject[sizeX, sizeY];
            for (var i = 0; i < sizeX; i++)
            {
                for (var j = 0; j < sizeY; j++)
                {
                    mNewGrid[i, j] = new EmptyObject(mGameState.mMissingTexture, i, j, grid);
                }
            }

            mOrigin = new Point(originX, originY);
        }

        //Adds any Object that inherits from AbstractGameObject to the Grid
        public void AddToGrid(int x, int y, AbstractGameObject gameObject)
        {
            if (mNewGrid != null && mNewGrid[x, y] is EmptyObject)
            {
                mNewGrid[x, y] = gameObject;
                switch (gameObject)
                {
                    case Human human:
                        Humans.Add(human);
                        break;
                    case ResourceObject resourceObject:
                        ResourceObjects.Add(resourceObject);
                        break;
                    case Campfire campfire:
                        Campfires.Add(campfire);
                        break;
                    case Wolf wolf:
                        Wolfs.Add((wolf));
                        break;
                    case Key key:
                        Keys.Add(key);
                        break;
                }
            }
        }

        public void AddNonInteractableObject(AbstractGameObject nonInteractableObject)
        {
            mNonInteractableGameObjects.Add(nonInteractableObject);
        }

        public void RemoveObjectFromGridPosition(Point pos)
        {
            var gameObject = GetAbstractGameObjectAt(pos);
            switch (gameObject)
            {
                case Human human:
                    Humans.Remove(human);
                    break;
                case ResourceObject resourceObject:
                    ResourceObjects.Remove(resourceObject);
                    break;
                case Campfire campfire:
                    Campfires.Remove(campfire);
                    break;
                case Wolf wolf:
                    Wolfs.Remove(wolf);
                    break;
                case Key key:
                    Keys.Remove(key);
                    break;
            }

            mNewGrid[pos.X, pos.Y] = new EmptyObject(mGameState.mMissingTexture, pos.X, pos.Y, this);
        }

        public AbstractGameObject GetAbstractGameObjectAtGridPosition(Vector2 position)
        {
            return mGrid[(int) position.X, (int) position.Y];
        }
        public AbstractGameObject GetAbstractGameObjectAtNewGridPosition(Vector2 position)
        {
            return mNewGrid[(int) position.X, (int) position.Y];
        }

        //Returns Position in the Grid from absolute Position
        public Vector2 GetGridPosition(Vector2 position)
        {
            return new Vector2((float) Math.Floor(position.X / FieldSize), (float) Math.Floor(position.Y / FieldSize));
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            foreach (var nonInteractableObject in mNonInteractableGameObjects)
            {
                nonInteractableObject.Draw(spritebatch, mCamera);
            }

            foreach (var abstractGameObject in mGrid)
            {
                abstractGameObject.Draw(spritebatch, mCamera);
            }
        }

        public void Update(GameTime gameTime, GameState gameState, SoundManager soundManager)
        {
            foreach (var abstractGameObject in mGrid)
            {
                abstractGameObject.Update(gameTime, mNewGrid, gameState, soundManager);
            }

            mGrid = new AbstractGameObject[mSizeX, mSizeY];
            for(int i = 0; i < mSizeX; i++)
            {
                for (int j = 0; j < mSizeY; j++)
                {
                    mGrid[i, j] = mNewGrid[i, j];
                }
            }
            
            // update keys so they can check if they are still on the map
            foreach (var keyObject in Keys)
            {
                keyObject.Update(gameTime, mNewGrid, gameState, soundManager);
            }

            foreach (var nonInteractableObject in mNonInteractableGameObjects)
            {
                nonInteractableObject.Update(gameTime, mNewGrid, gameState, soundManager);
            }
        }

        public List<AbstractGameObject> ReturnAllSaveableObjects()
        {
            var returnValue = new List<AbstractGameObject>();
            for (int i = 0; i < mSizeX; i++)
            {
                for (int j = 0; j < mSizeY; j++)
                {
                    if (!(mNewGrid[i, j] is EmptyObject))
                    {
                        returnValue.Add(mNewGrid[i, j]);
                    }
                }
            }

            foreach (var saveThis in mNonInteractableGameObjects)
            {
                returnValue.Add(saveThis);
            }

            return returnValue;
        }

        public Point GetGridOffset()
        {
            return mOrigin;
        }

        public AbstractGameObject GetAbstractGameObjectAt(Point p)
        {
            if (IsInGrid(p))
            {
                return mNewGrid[p.X, p.Y];
            }
            return null;
        }

        public bool IsInGrid(Point p)
        {
            return p.X >= 0 && p.X < mSize.X && p.Y >= 0 && p.Y < mSize.Y;
        }

        public Point PositionToGridCoordinates(Vector2 position)
        {
            var x = (int) (position.X / FieldSize);
            var y = (int) (position.Y / FieldSize);
            return new Point(x, y);
        }

        public Point PositionToGridCoordinatesRelativeToCamera(Vector2 position)
        {
            //Console.Out.WriteLine(position);
            position += mCamera.PositionPixels;
            return PositionToGridCoordinates(position);
        }
        public Point PositionToCoordinatesRelativeToCamera(Vector2 position)
        {
           // Console.Out.WriteLine(position);
            position += mCamera.PositionPixels;
            return new Point((int)position.X, (int)position.Y);
        }

        public Point GetCameraPosition()
        {
            return mCamera.PositionPixels.ToPoint();
        }

        public Generator GetGenerator()
        {
            return mGameState.GetGenerator();
        }
        public bool IsGridCoordinateInScreen(int x, int y)
        {
            var start = mCamera.PositionFields;
            var end = mCamera.GetScreenSizeFields();
            end.X += (int)mCamera.PositionFields.X;
            end.Y += (int)mCamera.PositionFields.Y;

            return x >= start.X && y >= start.Y && x < end.X && y < end.Y;
        }
    }
}
