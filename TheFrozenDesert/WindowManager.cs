using System;
using Microsoft.Xna.Framework;

namespace TheFrozenDesert
{
    internal sealed class WindowManager
    {
        private readonly GraphicsDeviceManager mGraphics;
        private readonly GameWindow mWindow;
        private int mHeight;
        private bool mIsBorderless;
        private bool mIsFullscreen;
        private int mWidth;

        public WindowManager(GraphicsDeviceManager graphics, GameWindow window)
        {
            mGraphics = graphics;
            mWindow = window;
        }

        private void ToggleFullscreen()
        {
            var oldIsFullscreen = mIsFullscreen;

            if (mIsBorderless)
            {
                mIsBorderless = false;
            }
            else
            {
                mIsFullscreen = !mIsFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }

        private void ApplyFullscreenChange(bool oldIsFullscreen)
        {
            if (mIsFullscreen)
            {
                if (oldIsFullscreen)
                {
                    ApplyHardwareMode();
                }
                else
                {
                    SetFullscreen();
                }
            }
            else
            {
                UnsetFullscreen();
            }
        }

        private void ApplyHardwareMode()
        {
            mGraphics.HardwareModeSwitch = !mIsBorderless;
            mGraphics.ApplyChanges();
        }

        private void SetFullscreen()
        {
            mWidth = mWindow.ClientBounds.Width;
            mHeight = mWindow.ClientBounds.Height;

            mGraphics.HardwareModeSwitch = !mIsBorderless;

            mGraphics.IsFullScreen = true;
            mGraphics.ApplyChanges();
        }

        private void UnsetFullscreen()
        {
            mGraphics.PreferredBackBufferWidth = mWidth;
            mGraphics.PreferredBackBufferHeight = mHeight;
            mGraphics.IsFullScreen = false;
            mGraphics.ApplyChanges();
        }

        public void OnToggleFullscreen(object source, EventArgs eventArgs)
        {
            ToggleFullscreen();
        }
    }
}