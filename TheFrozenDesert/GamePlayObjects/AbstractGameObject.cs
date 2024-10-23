using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Input;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public abstract class AbstractGameObject
    {
        #region Fields

        //Accurate current mPosition on the Map (Use for Drawing)
        protected Vector2 mPosition;
        #nullable enable
        protected AbstractGameObjectGui? mGui;

        protected readonly Grid mGrid;
        #nullable disable
        public struct GridPosition
        {
            internal GridPosition(int x, int y)
            {
                X = x;
                Y = y;
            }

            internal int Y { get; set; }
            internal int X { get; set; }
        }

        //Location of the Object in the Grid-Array
        protected GridPosition mGridPos;

        //Texture of the Object
        protected Texture2D mTexture;

        //Rectangle Size of the Object
        protected Rectangle Rectangle => new Rectangle((int) mPosition.X,
            (int) mPosition.Y, /*mTexture.Width, mTexture.Height*/
            32,
            32);

        protected Rectangle mTextureRegion = new Rectangle(0, 0, 32, 32);

        protected static int mFieldSize = 32;
        private bool mGuiOpen;
        protected bool mSelected;

        #endregion

        protected AbstractGameObject(Texture2D texture, int gridPosX, int gridPosY, Grid grid)
        {
            mTexture = texture;
            mGridPos.X = gridPosX;
            mGridPos.Y = gridPosY;
            mPosition = new Vector2((float) gridPosX * mFieldSize, (float) gridPosY * mFieldSize);
            mGrid = grid;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle(Rectangle.X - (int) camera.PositionPixels.X,
                Rectangle.Y - (int) camera.PositionPixels.Y,
                mFieldSize,
                mFieldSize);
            spriteBatch.Draw(mTexture, rect, mTextureRegion, color);
        }

        public void DrawGui(GameTime gameTime, SpriteBatch spriteBatch,GameState gameState)
        {
            if (mGuiOpen && !(mGui is null))
            {
                mGui.Draw(gameTime, spriteBatch,gameState);
            }
        }

        public abstract void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager);

        public virtual AbstractGameObjectModel Serialize(GameState gameState)
        {
            return null;
        }

        public Point GetGridPos()
        {
            return new Point(mGridPos.X, mGridPos.Y);
        }

        public GridPosition GetGridPosition()
        {
            return mGridPos;
        }

        public Point GetPos()
        {
            return new Point((int) mPosition.X - mGrid.GetCameraPosition().X, (int) mPosition.Y - mGrid.GetCameraPosition().Y);
        }

        public virtual void Select(List<AbstractGameObject> previouslySelectedObjects)
        {
            mSelected = true;
        }

        public void Unselect()
        {
            mSelected = false;
        }


        public Rectangle GetHitbox()
        {
            return Rectangle;
        }

        public bool GuiHitboxCheck(GameState gameState, InputHandler inputHandler)
        {
            if (!(mGui is null))
            {

                if(mGui.HitboxCheckRightClick(gameState, inputHandler))
                {
                    
                    gameState.mButtonsAbstractGameObjectGui.Clear();
                    gameState.mHasEnoughResourceForBuildingItems = "";
                    gameState.mHasEnoughResourceForBuildingSledgeStation = "";
                    gameState.mHasEnoughResourceForCookingFood = "";
                }
                return mGui.HitboxCheckLeftClick(gameState, inputHandler) || mGui.HitboxCheckRightClick(gameState, inputHandler);
            }

            return false;
        }

        public void OpenGui()
        {

            mGuiOpen = true;
        }

        public void CloseGui()
        {
            mGuiOpen = false;
        }

        public float DistanceTo(AbstractGameObject gameObject)
        {
            return (mPosition - new Vector2(gameObject.GetAbsolutePos().X, gameObject.GetAbsolutePos().Y)).Length();
        }

        public Point GetAbsolutePos()
        {
            return new Point((int)mPosition.X, (int)mPosition.Y);
        }
    }
}