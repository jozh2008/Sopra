using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public class ResourceObject : AbstractGameObject
    {
        public enum ResourceObjectType
        {
            Rock,
            Tree
        }

        public readonly ResourceObjectType mResourceObjectType;

        public int Earning { get; }

        public ResourceObject(GameState gameState, int gridPosX, int gridPosY, ResourceObjectType resourceObjectType, Grid grid) :
            base(gameState.mTreeRockSpriteSheet, gridPosX, gridPosY, grid)
        {
            switch (resourceObjectType)
            {
                case ResourceObjectType.Rock:
                    mTextureRegion = new Rectangle(0, 64, 32, 32);
                    Earning = 5;
                    break;
                case ResourceObjectType.Tree:
                    mTextureRegion = new Rectangle(0, 0, 32, 64);
                    Earning = 5;
                    break;

            }
            //mResourceType = resourceType;
            mResourceObjectType = resourceObjectType;
            //mGui = new ResourceObjectGui(gameState, this);
        }

        public ResourceObject(GameState gameState, ResourceObjectModel resourceObjectModel, Grid grid) : base(
            gameState.mTreeRockSpriteSheet,
            resourceObjectModel.X - gameState.GetGridOffset().X,
            resourceObjectModel.Y - gameState.GetGridOffset().Y,
            grid)
        {
            if (resourceObjectModel.Type.Equals("Rock"))
            {
                mResourceObjectType = ResourceObjectType.Rock;
                mTextureRegion = new Rectangle(0, 64, 32, 32);
                mGui = new ResourceObjectGui(gameState, this);
                Earning = 5;
                return;
            }
            Earning = 5;
            mResourceObjectType = ResourceObjectType.Tree;
            mTextureRegion = new Rectangle(0, 0, 32, 64);
            //mGui = new ResourceObjectGui(gameState, this);
        }

       

        public ResourceType ReturnResourceType
        {
            get
            {
                if (mResourceObjectType == ResourceObjectType.Tree)
                {
                    return ResourceType.Wood;
                }

                if (mResourceObjectType == ResourceObjectType.Rock)
                {
                    return ResourceType.Metall;
                }

                return ResourceType.Null; // if no valid type return empty string
            }
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle(Rectangle.X - (int) camera.PositionPixels.X,
                Rectangle.Y - (int) camera.PositionPixels.Y + (mFieldSize - mTextureRegion.Height), //it needs to be drawn as if it were 1 square higher so it can be selected from bottom and be drawn from top (stupidfix)
                mFieldSize,
                mFieldSize*(mTextureRegion.Height)/mFieldSize);             //if the to draw sprite is2 high (like tree) it needs to be a 32*64 rectangle
            spriteBatch.Draw(mTexture, rect, mTextureRegion, color);
        }

        public string ReturnResourceObjectTypeAsString()
        {
            return mResourceObjectType.ToString();
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var resourceObjectModel = new ResourceObjectModel();
            resourceObjectModel.X = mGridPos.X + gameState.GetGridOffset().X;
            resourceObjectModel.Y = mGridPos.Y + gameState.GetGridOffset().Y;
            switch (mResourceObjectType)
            {
                case ResourceObjectType.Rock:
                    resourceObjectModel.Type = "Rock";
                    break;
                case ResourceObjectType.Tree:
                    resourceObjectModel.Type = "Tree";
                    break;
            }

            return resourceObjectModel;
        }
    }
}