using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;

namespace TheFrozenDesert.Input
{
    public class SelectionManager
    {
        private Rectangle mSelectionRectangle;
        private readonly Color mSelectionColor = new Color(Color.Blue, 0.5f);

        private bool mIsGroupSelecting;

        private List<AbstractGameObject> mSelectedObjects;

        public SelectionManager()
        {
            mSelectedObjects = new List<AbstractGameObject>();
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (mIsGroupSelecting)
            {
                spriteBatch.DrawRectangle(mSelectionRectangle, mSelectionColor); 
            }
        }

        private void UnselectAll()
        {
            // unselect items in list
            foreach (AbstractGameObject abstractGameObject in mSelectedObjects)
            {
                abstractGameObject.Unselect();
            }
            // remove all elemnts from List
            mSelectedObjects = new List<AbstractGameObject>();
        }

        // Update
        public void Update(GameTime gameTime, Game1.Managers managers, GameState gameState)
        {
            UpdateInput(managers, gameState);
            
        }

        private bool IsSelectionObject(AbstractGameObject gameObject)
        {
            if (gameObject is Human {Faction: EFaction.Player} || gameObject is Sledge)
            {
                return true;
            }

            return false;

        }

        private void CheckLeftClick(GameState gameState, Game1.Managers managers)
        {
            gameState.mGuiOpen?.CloseGui(); // close all guis

            if (!managers.mInputHandler.ControlIsDown)
            {
                UnselectAll();
            }
            
            var gridDestination = gameState.mGrid.PositionToGridCoordinatesRelativeToCamera(managers.mInputHandler.MousePosition);
            var selectedObject = false;
            for (var i = -1; i <= 1; i++)
            {
                if (selectedObject)
                {
                    break;
                }

                for (var j = -1; j <= 1; j++)
                {
                    var placePosition = new Point(gridDestination.X + i, gridDestination.Y + j);
                    if (gameState.mGrid.IsInGrid(placePosition) && IsSelectionObject(gameState.mGrid.GetAbstractGameObjectAt(placePosition)))
                    {
                        var abstractGameObject = gameState.mGrid.GetAbstractGameObjectAt(placePosition);
                        if (managers.mInputHandler.Inputs.IsSelectedInGrid(abstractGameObject.GetHitbox(),
                                gameState.mGrid))
                        {
                            List<AbstractGameObject> previouslySelected = mSelectedObjects;

                            abstractGameObject.Select(previouslySelected);
                            mSelectedObjects.Add(abstractGameObject);
                            selectedObject = true;
                            InputHandler.InputList.BlockFurtherInput();
                            break;
                        }
                        
                    }
                }
            }
            
        }
        private void UpdateInput(Game1.Managers managers, GameState gameState)
        {
            mIsGroupSelecting = managers.mInputHandler.IsSelection;
            if (mIsGroupSelecting)
            {
                mSelectionRectangle = managers.mInputHandler.Inputs.GetSelectionRectangle();
                InputHandler.InputList.BlockFurtherInput();
            }
            else if (managers.mInputHandler.MouseClickLeft)
            {
                CheckLeftClick(gameState, managers);
            }
            else if (managers.mInputHandler.Select)
            {
                List<AbstractGameObject> previouslySelected = mSelectedObjects;
                
                SelectGroup(gameState, managers, previouslySelected);
            }
        }

        private void SelectGroup(GameState gameState, Game1.Managers managers, List<AbstractGameObject> previouslySelected)
        {
            if (!managers.mInputHandler.ControlIsDown)
            {
                UnselectAll(); // if selection is empty all objects should be unselected
            }

            // check which GamePlayObjects are in the selectedRectangle and select them
            var startPoint = gameState.mGrid.GetGridPosition(new Vector2(mSelectionRectangle.X + gameState.mGrid.GetCameraPosition().X, mSelectionRectangle.Y + gameState.mGrid.GetCameraPosition().Y));
            var endPoint =
                gameState.mGrid.GetGridPosition(new Vector2(mSelectionRectangle.Right + 1 + gameState.mGrid.GetCameraPosition().X, mSelectionRectangle.Bottom + 1 + gameState.mGrid.GetCameraPosition().Y));
            bool refreshed = false;
            // iterate trough Grid field
            for (int x = (int) startPoint.X; x <= endPoint.X; x++)
            {
                for (int y = (int) startPoint.Y; y <= endPoint.Y; y++)
                {
                    var placePosition = new Point(x, y);
                    if(gameState.mGrid.IsInGrid(placePosition) && IsSelectionObject(gameState.mGrid.GetAbstractGameObjectAt(placePosition)))
                    {
                        if (!refreshed)
                        {
                            refreshed = true;
                        }
                        var abstractGameObject = gameState.mGrid.GetAbstractGameObjectAt(placePosition);
                        if (managers.mInputHandler.Inputs.InGroupSelectionInGrid(abstractGameObject.GetHitbox(), gameState.mGrid))
                        {
                            abstractGameObject.Select(previouslySelected);
                            mSelectedObjects.Add(abstractGameObject);
                        }
                        
                    }
                }
            }
            
        }

        public void SelectObject(AbstractGameObject abstractGameObject)
        {
            abstractGameObject.Select(mSelectedObjects);
            mSelectedObjects.Add(abstractGameObject);
        }
    }
}