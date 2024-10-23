using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheFrozenDesert.GamePlayObjects;

namespace TheFrozenDesert.Input
{
    public sealed class InputHandler
    {
        public delegate void ToggleFullScreenEventHandler(object source, EventArgs args);

        private static bool sInputIsBlocked;

        internal InputHandler()
        {
            mMousePressedLeft = false;
            mMousePressedRight = false;
            MousePositionPressStart = new Vector2(0, 0);
            Update();
        }

        public struct InputList
        {
            private bool MouseJustReleasedLeft { get; }
            private bool MouseClickLeft { get; }
            private bool MouseClickRight { get;}
            private Vector2 MousePosition { get;}
            private Rectangle Selection { get;}

            internal InputList(bool mouseClickLeft, bool mouseClickRight, Vector2 mousePosition, bool mouseJustReleasedLeft, Rectangle selection)
            {
                MouseClickLeft = mouseClickLeft;
                MouseClickRight = mouseClickRight;
                MousePosition = mousePosition;
                MouseJustReleasedLeft = mouseJustReleasedLeft;
                Selection = selection;
            }

            internal static void BlockFurtherInput()
            {
                sInputIsBlocked = true;
            }

            internal bool IsSelected(Rectangle rectangle)
            {
                return MouseClickLeft && rectangle.Contains(MousePosition) && !sInputIsBlocked;
            }

            internal bool IsSelectedInGrid(Rectangle rectangle, Grid grid)
            {
                return MouseClickLeft && rectangle.Contains(grid.PositionToCoordinatesRelativeToCamera(MousePosition)) && !sInputIsBlocked;
            }

            internal bool IsOpeningGui(Rectangle rectangle)
            {
                return MouseClickRight && rectangle.Contains(MousePosition) && !sInputIsBlocked;
            }

            public bool IsOpeningGuiInGrid(Rectangle rectangle, Grid grid)
            {
                return MouseClickRight && rectangle.Contains(grid.PositionToCoordinatesRelativeToCamera(MousePosition)) && !sInputIsBlocked;
            }

            internal Rectangle GetSelectionRectangle()
            {
                return Selection;
            }

            internal bool InGroupSelectionInGrid(Rectangle rectangle, Grid grid)
            {
                Rectangle localRectangle = new Rectangle(rectangle.Location - grid.GetCameraPosition(), rectangle.Size);
                return (sSelect && Selection.Intersects(localRectangle)) && !sInputIsBlocked;
            }

        }

        internal InputList Inputs { get;  private set; }
        private MouseState Mouse { get; set; }
        
        private bool mMousePressedLeft;

        internal bool MousePressedLeft => mMousePressedLeft && !sInputIsBlocked;

        private bool mMouseJustReleasedLeft;
        internal bool MouseJustReleasedLeft => mMouseJustReleasedLeft;
        
        private bool mMousePressedRight;
        private bool mMouseClickLeft;
        internal bool MouseClickLeft => mMouseClickLeft && !sInputIsBlocked;

        private bool mMouseClickRight;
        internal bool MouseClickRight => mMouseClickRight && !sInputIsBlocked;

        private bool mIsSelection;
        internal bool IsSelection => mIsSelection && !sInputIsBlocked;

        private static bool sSelect;
        internal bool Select => sSelect && !sInputIsBlocked;

        private bool mControlIsDown;
        internal bool ControlIsDown => mControlIsDown && !sInputIsBlocked;

        private bool mUIsDown;
        internal bool UIsDown => mUIsDown && !sInputIsBlocked;

        private bool mKeyboardRIsDown;
        internal bool KeyboardRIsDown => mKeyboardRIsDown && !sInputIsBlocked;

        private bool mSkillKeyDown;
        internal bool SkillKeyDown => mSkillKeyDown && !sInputIsBlocked;
        internal Vector2 MousePosition { get; private set; }
        private Vector2 MousePositionPressStart { get; set; }
        public event ToggleFullScreenEventHandler ToggleFullscreen;

        internal void Update()
        {
            sInputIsBlocked = false;
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            MousePosition = new Vector2(Mouse.X, Mouse.Y);
            
            // left mouse button
            mMouseClickLeft = false;
            sSelect = false;
            // check if it was a selection or a click
            var diff = new Vector2(Math.Abs(MousePositionPressStart.X - MousePosition.X), Math.Abs(MousePositionPressStart.Y - MousePosition.Y));
            if ((diff.X > 10 || diff.Y > 10) && mMousePressedLeft)
            {
                // selection
                mIsSelection = true;
            }
            
            if (Mouse.LeftButton == ButtonState.Pressed && mMousePressedLeft == false)
            {
                mMousePressedLeft = true;
                MousePositionPressStart = MousePosition;
            }

            if (Mouse.LeftButton == ButtonState.Released && mMousePressedLeft) // left button released
            {
                mMousePressedLeft = false;
                // check if it was a click or a selection
                if (!mIsSelection)
                {
                    mMouseClickLeft = true;
                }
                else // selection
                {
                    sSelect = true;
                }

                mIsSelection = false;
            }


            if (Mouse.RightButton == ButtonState.Pressed && mMousePressedRight == false)
            {
                mMousePressedRight = true;
                mMouseClickRight = true;
            }
            else
            {
                mMouseClickRight = false;
            }

            if (Mouse.LeftButton == ButtonState.Released && mMousePressedLeft)
            {
                mMousePressedLeft = false;
                mMouseJustReleasedLeft = true;
            }
            else
            {
                mMouseJustReleasedLeft = false;
            }

            if (Mouse.RightButton == ButtonState.Released)
            {
                mMousePressedRight = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                OnToggleFullscreen();
            }

            mKeyboardRIsDown = Keyboard.GetState().IsKeyDown(Keys.R);

            mControlIsDown = Keyboard.GetState().IsKeyDown(Keys.LeftControl);

            mUIsDown = Keyboard.GetState().IsKeyDown(Keys.U);

            mSkillKeyDown = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            

            Rectangle selectionRectangle = new Rectangle(
                (int) Math.Min(MousePositionPressStart.X, MousePosition.X),
                (int) Math.Min(MousePositionPressStart.Y, MousePosition.Y),
                Math.Abs((int) MousePosition.X - (int)MousePositionPressStart.X),
                Math.Abs((int) MousePosition.Y - (int)MousePositionPressStart.Y));
            Inputs = new InputList(mMouseClickLeft, mMouseClickRight, MousePosition, mMouseJustReleasedLeft, selectionRectangle);
        }

        private void OnToggleFullscreen()
        {
            if (ToggleFullscreen != null)
            {
                ToggleFullscreen(this, null);
            }
        }
    }
}