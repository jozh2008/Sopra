using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GenerateMap
{
    internal sealed class MapGenerator
    {
        private const int OctaveCount = 6;
        private readonly GameState mGameState;
        private readonly Grid mGrid;
        private readonly GamePlayObjectFactory mGamePlayObjectFactory;

        // define bounds

        private readonly PerlinNoise mPerlinNoiseGen;
        private readonly TiledMapTileLayer mTileLayerDrawable;
        private readonly TiledMapTileLayer mTileLayerBlocked;

        private readonly Random mRandom;

        public MapGenerator(TiledMap tiledMap, Grid grid, GameState gameState, GamePlayObjectFactory gamePlayObjectFactory)
        {
            var tileLayerDrawable = tiledMap.GetLayer<TiledMapTileLayer>("Drawable");
            var tileLayerBlocked = tiledMap.GetLayer<TiledMapTileLayer>("Wall");
            if (tileLayerDrawable == null || tileLayerBlocked == null)
            {
                throw new Exception("TiledMap Layer doesent Exist");
            }

            mGrid = grid;
            mGameState = gameState;
            mRandom = new Random(gameState.GetGameStateId());
            mTileLayerDrawable = tileLayerDrawable;
            mTileLayerBlocked = tileLayerBlocked;
            mGamePlayObjectFactory = gamePlayObjectFactory;

            mPerlinNoiseGen = new PerlinNoise();
        }


        public void GenerateNeutralPlayers(int quantity)
        {
            for(int i = 0; i< quantity; i++)
            {
                var r = new Random();
                var x = r.Next(0, mGrid.Size.X);
                var y = r.Next(0, mGrid.Size.Y);
                if (IsTileFree((ushort)x, (ushort) y, mTileLayerDrawable)) {
                    if (i % 3 == 0)
                    {

                        mGamePlayObjectFactory.CreateArcher(x, y, EFaction.Neutral, AI.Routine.Neutral);
                    }
                    else if(i%3 == 1)
                    {
                        mGamePlayObjectFactory.CreateGatherer(x, y, EFaction.Neutral, AI.Routine.Neutral);
                    }
                    else if( i%3 == 2)
                    {
                        mGamePlayObjectFactory.CreateFighter(x, y, EFaction.Neutral, AI.Routine.Neutral);
                    }
                }
            }
        }

        public void GenerateObjects(List<ResourceObject.ResourceObjectType> elements, double power, double subtract)
        {
            // Generate PerlinNoise for every field of the Grid
            var perlinNoise = mPerlinNoiseGen.GeneratePerlinNoise(mGrid.Size.X, mGrid.Size.Y, OctaveCount);
            for (var i = 0; i < mGrid.Size.X; i++)
            {
                for (var j = 0; j < mGrid.Size.Y; j++)
                {
                    for (var k = 0; k < elements.Count; k++)
                    {
                        if (!(mRandom.NextDouble() < Math.Pow(perlinNoise[i][j] - subtract, power)))
                        {
                            continue;
                        }
                        
                        if (IsTileFree((ushort)(i), (ushort)(j),mTileLayerDrawable) &&
                            IsGridFree(mGrid.GetAbstractGameObjectAtGridPosition(new Vector2(i, j))))
                        {
                            mGrid.AddToGrid(i, j, new ResourceObject(mGameState, i, j, elements[k], mGrid));
                        }
                    }
                }
            }
        }

        public void GenerateImportantWolfPack()
        {
            int randomXPositionEastOfMap = mRandom.Next(200, 350);
            int randomYPosition = mRandom.Next(0, 30);
            
            
            
            mGamePlayObjectFactory.CreateImportantWolfPack(randomXPositionEastOfMap,randomYPosition);
        }

        public void GenerateWolfPacks(int numberOfWolfpacks)
        {
            var numberOfWolves = mRandom.Next(2, 5);
            for(int i = 0; i < numberOfWolfpacks; i++)
            {
                var x = mRandom.Next(67, mGrid.Size.X - 100);
                var y = mRandom.Next(10, mGrid.Size.Y - 20);
                if (!IsTileFree((ushort) x, (ushort) y, mTileLayerBlocked))
                {
                    mGamePlayObjectFactory.CreateWolfPack(x, y, numberOfWolves);
                }
            }
            
        }

        public void PlaceBlockFields()
        {
            for (var x = 0; x < mGrid.Size.X; x++)
            {
                for (var y = 0; y < mGrid.Size.Y; y++)
                {
                    if (IsTileFree((ushort) x, (ushort) y, mTileLayerBlocked))
                    {
                        mGamePlayObjectFactory.CreateBlockTile(x, y);
                    }
                }
            }

        }

        private bool IsTileFree(ushort posx, ushort posy, TiledMapTileLayer tiledMapTileLayer)
        {
            tiledMapTileLayer.TryGetTile(posx, posy, out var tile);

            return tile is {IsBlank: false};
        }

        private bool IsGridFree(AbstractGameObject gameObject)
        {
            if (gameObject is EmptyObject)
            {
                return true;
            }

            return false;
        }
    }
}