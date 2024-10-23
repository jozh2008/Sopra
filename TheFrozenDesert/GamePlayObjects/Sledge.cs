using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.MainMenu.SubMenu;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{

    public class Sledge : AbstractMoveableObject
    {
        public Guid Uuid { get;}

        private int mWoodForSledgeTypeOtherThanDampfmaschine = 10;
        private int mRockForSedgeTypeOtherThanDampfmaschine = 5;

        private int mWoodForDampfmaschine = 40;
        private int mRockForDampfmaschine = 15;
        private double mTimeSinceLastMovement;
        public int RockForSedgeTypeOtherThanDampfmaschine => mRockForSedgeTypeOtherThanDampfmaschine;
        public int WoodForSledgeTypeOtherThanDampfmaschine => mWoodForSledgeTypeOtherThanDampfmaschine;

        protected Sledge(
#nullable enable
            Texture2D texture,
            int gridPosX,
            int gridPosY,
            float movementSpeed,
            Sledge nextSledge,
            Grid grid
#nullable disable
        ) : base(
            texture,
            gridPosX,
            gridPosY,
            movementSpeed,
            grid
        )
        {
            mNextSledge = nextSledge;
            Uuid = Guid.NewGuid();
            //mGui = new SledgeGui(gameState, this);
        }

        public Sledge(GameState gameState,
            Texture2D texture,
            SledgeModel sledgeModel,
            float movementSpeed,

            Grid grid) : base(
            texture,
            sledgeModel.X,
            sledgeModel.Y,
            movementSpeed,
            grid)
        {
            Uuid = new Guid(sledgeModel.Uuid);
            mGui = new SledgeGui(gameState, this);
        }

    public override void Update(GameTime gameTime,
        AbstractGameObject[,] grid,
        GameState gameState,
        SoundManager soundManager)
        {
            base.Update(gameTime, grid, gameState, soundManager);
            mTimeSinceLastMovement += gameTime.ElapsedGameTime.TotalSeconds;
            if (IsMoving() && mTimeSinceLastMovement > 1)
            {
                mTimeSinceLastMovement = 0;
                gameState.mManagers.mSoundManager.Sledge_Sliding();
                foreach (Human human in gameState.mGrid.Humans)
                {
                    if (human.Faction == EFaction.Player && human.DistanceTo(this) >= 500)
                    {
                        human.SetDestinationObject(this);
                    }
                }
            }

            if (!IsMoving() && IsFront() && mSelected && gameState.mHasDampfmaschine) // need to habe Dampfaschine to move, at the begin we don't have one
            {

                if ((gameState.mResources.Get(ResourceType.Wood) >= 1))
                {
                    
                    var text = "";
                    gameState.mHasEnoughWoodForMovingSledge = text;
                    if (gameState.mInputHandler.MouseClickRight)
                    {

                        var gridDestination =
                            PositionToGridCoordinatesRelativeToCamera(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                        mGui = null; // if moving,then sledgemenu should be closed
                        SetGridTarget(gridDestination.X, gridDestination.Y);
                        if (gridDestination.X != mGridPos.X || gridDestination.Y != mGridPos.Y)// not same posion  then dampfmaschine needs wood to move
                        {
                            gameState.mResources.Decrease(ResourceType.Wood, 1);
                        }
                    }
                }
                else
                {
                    var text = "Braucht 1 Holzstück zum Schlittenbewegen";
                    gameState.mHasEnoughWoodForMovingSledge = text;
                }
            }
            // can only open sledge menu if human is next to sledge
            mGui = IsNeigbourToHuman(GetGridPos(), gameState) ? new SledgeGui(gameState, this) : null;
        }

        private bool IsFront()
        {
            return mNextSledge == null;
        }

        protected override void PostSwitchGridPosition(Point previousPosition)
        {
            if (mPreviousSledge != null)
            {
                mPreviousSledge.SetGridTarget(previousPosition.X, previousPosition.Y);
            }
        }

        public bool TryAttachSledge(Grid grid)
        {
            if (mPreviousSledge == null)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var placePosition = new Point(GetGridPos().X + i, GetGridPos().Y + j);
                        if (grid.IsInGrid(placePosition) && grid.GetAbstractGameObjectAt(placePosition) is
                            EmptyObject)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return mPreviousSledge.TryAttachSledge(grid);
        }


        private bool TryAttachSledgeinFront(Grid grid)
        {
            if (mNextSledge == null)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var placePosition = new Point(GetGridPos().X + i, GetGridPos().Y + j);
                        if (grid.IsInGrid(placePosition) && grid.GetAbstractGameObjectAt(placePosition) is
                            EmptyObject)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return mPreviousSledge != null && mPreviousSledge.TryAttachSledge(grid);
        }

        // tryattach dampfmaschine
        internal void AttachSledgeFront(GameState gameState, Grid grid)
        {
            if (mNextSledge == null)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var placePosition = new Point(GetGridPos().X + i, GetGridPos().Y + j);
                        if (grid.IsInGrid(placePosition) && grid.GetAbstractGameObjectAt(placePosition) is
                            EmptyObject)
                        {
                            var newSledge = new SteamEngine(mTexture,
                                placePosition.X,
                                placePosition.Y,
                                100,
                                null,
                                grid);
                            gameState.AddToGrid(placePosition.X, placePosition.Y, newSledge);
                            newSledge.mPreviousSledge = this;
                            mNextSledge = newSledge;
                            gameState.mHasDampfmaschine = true;
                            return;
                        }
                    }
                }
            }
        }
        
        // tryattch other sledge type 
        public bool TryAttachSledgeType(GameState gameState, Grid grid,string type)
        {
            if (mPreviousSledge == null)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var placePosition = new Point(GetGridPos().X + i, GetGridPos().Y + j);
                        if (grid.IsInGrid(placePosition) && grid.GetAbstractGameObjectAt(placePosition) is
                            EmptyObject)
                        {
                            var newSledge = ReturnNewStation(type, gameState, placePosition, grid);
                           
                                gameState.AddToGrid(placePosition.X, placePosition.Y, newSledge);
                                mPreviousSledge = newSledge;
                            
                            
                            return true;
                        }
                    }
                }

                return false;
            }

            return mPreviousSledge.TryAttachSledgeType(gameState, grid,type);
        }

        public bool CanBuildSledgeType(GameState gameState)
        {
            if((gameState.mResources.Get(ResourceType.Wood) >= mWoodForSledgeTypeOtherThanDampfmaschine 
                && gameState.mResources.Get(ResourceType.Metall)>= mRockForSedgeTypeOtherThanDampfmaschine
                && TryAttachSledge(mGrid)))
            {
                return true;
            }
            return false;
        }

        public void BuildSledgeType(GameState gameState)
        {
            if (CanBuildSledgeType(gameState))
            {
                gameState.mResources.Decrease(ResourceType.Wood,mWoodForSledgeTypeOtherThanDampfmaschine);
                gameState.mResources.Decrease(ResourceType.Metall,mRockForSedgeTypeOtherThanDampfmaschine);
            }
          
        }

        public void BuildDampfmaschine(GameState gameState)
        {
            if (CanBuildDampfmaschine(gameState))
            {

                gameState.mResources.Decrease(ResourceType.Wood,  mWoodForDampfmaschine);
                gameState.mResources.Decrease(ResourceType.Metall,mRockForDampfmaschine);
               
            }

        }

        public bool CanBuildDampfmaschine(GameState gameState)
        {
            if (gameState.mResources.Get(ResourceType.Wood) >= mWoodForDampfmaschine 
                && gameState.mResources.Get(ResourceType.Metall) >= mRockForDampfmaschine
                && !gameState.mHasDampfmaschine
                && TryAttachSledgeinFront(mGrid))
            {
                return true;
            }
            return false;
        }


        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var sledgeModel = new PlainSledgeModel
            {
                X = mGridPos.X + gameState.GetGridOffset().X,
                Y = mGridPos.Y + gameState.GetGridOffset().Y,
                PreviousSledgeUuid = mPreviousSledge is null ? "none" : mPreviousSledge.Uuid.ToString(),
                Uuid = Uuid.ToString()
            };
            return sledgeModel;
        }

        private Sledge ReturnNewStation(string station,GameState gameState, Point placePosition,Grid grid)
        {
            if (station== "Unterkunft")
            {
                var stationUnterkunt =new Unterkunft(mTexture,
                    placePosition.X,
                    placePosition.Y,
                    100,
                    this,
                    grid);
                stationUnterkunt.AddCapacity(gameState);// if new unterkunft, then more capacity
                return  stationUnterkunt;
            }
            else if(station == "Kamin")
            {
                gameState.mHasKamin = true;
                return new Kamin(mTexture,
                    placePosition.X,
                    placePosition.Y,
                    100,
                    this,
                    grid);
            }
            else if(station == "Küche")
            {
                return new Kitchen(mTexture,
                    placePosition.X,
                    placePosition.Y,
                    100,
                    this,
                    grid);
            }
            else if(station == "Hospiz")
            {
                return new Hospiz(mTexture,
                   placePosition.X,
                   placePosition.Y,
                   100,
                   this,
                   grid);
            }
            else if(station == "Werkbank")
            {
                return new Workbench(mTexture,
                   placePosition.X,
                   placePosition.Y,
                   100,
                   this,
                   grid);
            }
            else if (station == "Schmiede")
            {
                return new Forge(mTexture,
                   placePosition.X,
                   placePosition.Y,
                   100,
                   this,
                   grid);
            }
            else if (station == "Lager")
            {
                return new Stock(mTexture,
                   placePosition.X,
                   placePosition.Y,
                   100,
                   this,
                   grid);
            }
            return null;
        }
        public void SetPreviousSledge(Sledge sledge)
        {
            mPreviousSledge = sledge;
            sledge.mNextSledge = this;
        }

        private bool IsNeigbourToHuman(Point gridPosition1, GameState gameState)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    Point human = new Point(gridPosition1.X + i, gridPosition1.Y + j);
                    if (human.X >= 0 && human.Y >= 0)
                    {
                        var gameObject = gameState.mGrid.GetAbstractGameObjectAt(human);

                        if (gameObject is Human)
                        {
                            gameState.mHumanPosition = human;
                            return true;
                        }
                       
                    }
                }
            }
            return false;
        }

#nullable enable
        private Sledge? mNextSledge;
        protected Sledge? mPreviousSledge;
#nullable disable
    }
}