using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;


namespace TheFrozenDesert.MainMenu.SubMenu
{
    internal class Kamin: Sledge
    {
        private readonly int mRange = 3;
        float mTimer;
        public Kamin(Texture2D texture, int gridPosX, int gridPosY, float movementSpeed, Sledge nextSledge, Grid grid) : base(texture, gridPosX, gridPosY, movementSpeed, nextSledge, grid)
        {
            mTextureRegion = new Rectangle(32, 96, 32, 32);
        }

        public Kamin(OvenModel model,
            GameState gameState,
            Texture2D texture2D,
            float movementSoeed,
            Grid grid) :
            base(gameState, texture2D, model, movementSoeed, grid)
        {
            mTextureRegion = new Rectangle(32, 96, 32, 32);
        }
        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var sledgeModel = new OvenModel()
            {
                X = mGridPos.X + gameState.GetGridOffset().X,
                Y = mGridPos.Y + gameState.GetGridOffset().Y,
                PreviousSledgeUuid = mPreviousSledge is null ? "none" : mPreviousSledge.Uuid.ToString(),
                Uuid = Uuid.ToString()
            };
            return sledgeModel;
        }
        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)

        {
            base.Update(gameTime, grid, gameState, soundManager);
            if (gameState.mKaminIsActivated)
            {


                IsinsideKaminRange(this.GetGridPos(), gameState, mRange);


                if (gameState.mResources.Get(ResourceType.Wood) >= 1)
                {
                    mTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (mTimer >= 5000) // Your time, in milliseconds
                    {
                        gameState.mResources.Decrease(ResourceType.Wood,1);
                        mTimer -= 5000;
                    }
                }
                else
                {
                    gameState.mKaminIsActivated = false;
                }
            }

        }


        private void IsinsideKaminRange(Point sledge, GameState gameState, int range)
        {
            var gameObjectSledge = gameState.mGrid.GetAbstractGameObjectAt(sledge);
            if (gameObjectSledge is Kamin)
            {
                for (var i = -range; i <= range; i++)
                {
                    for (var j = -range; j <= range; j++)
                    {
                        Point human = new Point(sledge.X + i, sledge.Y + j);
                        if (human.X >= 0 && human.Y >= 0)
                        {
                            var gameObject = gameState.mGrid.GetAbstractGameObjectAt(human);
                           
                            if (gameObject is Human && gameState.mResources.Get(ResourceType.Wood) >= 1)
                            {
                                
                                Human humanObject = (Human)gameObject;
                              
                                    //Debug.WriteLine(humanObject.Health + " " + humanObject.GetGridPos());
                                    humanObject.AddHeat(1);
                                    
                               
                            }

                        }
                    }
                }
            }
        }
    }
}

