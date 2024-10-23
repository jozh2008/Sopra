using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace TheFrozenDesert.GamePlayObjects
{
    public class Camera
    {
        private const int VirtualWidthPixels = 1280; // can be divided by 32
        private const int VirtualHeightPixels = 720; // format = 16:9
        private const int MapWidthFields = 575; // in Fields
        private const int MapHeightFields = 177; // in Fields
        private const int FieldSize = 32;
        private readonly OrthographicCamera mCamera;

        private readonly Vector2 mStartPositionPixels =
            new Vector2((float) VirtualWidthPixels / 2, (float) VirtualHeightPixels / 2);

        private bool mCentering; // bool to see if centering is active

        private AbstractMoveableObject mCenterObject; // Object to center the Camera to
        private int mMovmentSpeed = 200; // speed for Camera movement
        private Vector2 mPositionPixels;

        private Vector2 mTargetPositionPixels;


        public Camera(GraphicsDevice graphicsDevice, GameWindow window)
        {
            var viewportadapter =
                new BoxingViewportAdapter(window, graphicsDevice, VirtualWidthPixels, VirtualHeightPixels);
            mCamera = new OrthographicCamera(viewportadapter);
            mPositionPixels = mStartPositionPixels;
            mTargetPositionPixels = mPositionPixels;
            mCamera.LookAt(mPositionPixels);
        }

        public Matrix GetView => mCamera.GetViewMatrix();

        public Vector2 PositionFields =>
            // returns Position of Field in top left corner of the window
            PositionPixelToField(mPositionPixels);

        public Vector2 PositionPixels =>
            // returns Position in Pixels relative to start position
            mPositionPixels - mStartPositionPixels;

        public void Update_Cam(GameTime gameTime)
        {
            var distance = Vector2.Distance(mPositionPixels, mTargetPositionPixels);
            if (distance > 1)
            {
                // move Camera to target position
                mPositionPixels += GetMovmentDirection() * mMovmentSpeed *
                                   (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                mPositionPixels = mTargetPositionPixels;
            }

            mCamera.LookAt(mPositionPixels);
            UpdateCentering();
        }

        private void MoveCameraAbsolute(Vector2 targetPositionField, int movementSpeed)
        {
            SetTargetPosition(PositionFieldToPixels(targetPositionField));
            mMovmentSpeed = movementSpeed;
        }

        public void ActivateCenterObjekt(AbstractMoveableObject gameObject)
        {
            mCenterObject = gameObject;
            mMovmentSpeed = (int) mCenterObject.MovementSpeed;
            mCentering = true;
        }


        private void SetTargetPosition(Vector2 position)
        {
            var endPositionPixels = new Vector2(
                MapWidthFields * FieldSize - VirtualWidthPixels + mStartPositionPixels.X,
                MapHeightFields * FieldSize - VirtualWidthPixels + mStartPositionPixels.Y);

            // set x if it is valid
            if (position.X >= mStartPositionPixels.X &&
                position.X <= endPositionPixels.X)
            {
                mTargetPositionPixels.X = position.X;
            }
            else
            {
                mTargetPositionPixels.X = mPositionPixels.X;
            }

            // set y if it is valid
            if (position.Y >= mStartPositionPixels.Y &&
                position.Y <= endPositionPixels.Y)
            {
                mTargetPositionPixels.Y = position.Y;
            }
            else
            {
                mTargetPositionPixels.Y = mPositionPixels.Y;
            }
        }

        private Vector2 GetMovmentDirection()
        {
            var direction = mTargetPositionPixels - mPositionPixels;
            direction.Normalize();
            return direction;
        }

        private Vector2 PositionPixelToField(Vector2 pixel)
        {
            pixel.X -= mStartPositionPixels.X;
            pixel.Y -= mStartPositionPixels.Y;

            pixel.X /= FieldSize;
            pixel.Y /= FieldSize;

            return pixel;
        }

        private Vector2 PositionFieldToPixels(Vector2 posField)
        {
            // Convert absolute Camera Field position to Pixelposition
            posField.X *= FieldSize;
            posField.Y *= FieldSize;

            posField += mStartPositionPixels;

            return posField;
        }

        private void UpdateCentering()
        {
            if (mCentering)
            {
                // Update camera position so the Object is in the middle of the screen
                var objectPositionFields = new Vector2(mCenterObject.GetGridPos().X, mCenterObject.GetGridPos().Y);
                var height = VirtualHeightPixels / 2 / FieldSize;
                var width = VirtualWidthPixels / 2 / FieldSize;
                var diffToMiddleField =
                    new Vector2(width, height) - objectPositionFields;
                MoveCameraAbsolute(-diffToMiddleField, mMovmentSpeed);
            }
        }

        public Point GetScreenSizeFields()
        {
            return new Point((VirtualWidthPixels / FieldSize) , (VirtualHeightPixels /FieldSize));
        }

        public Point GetScreenSize()
        {
            return new Point(VirtualWidthPixels , VirtualHeightPixels);
        }
    }
}