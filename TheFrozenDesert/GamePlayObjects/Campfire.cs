using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
   public sealed class Campfire : AbstractGameObject
    {
        private float mTimeToLive;
        private bool mIsDead; //fire is dead
        private readonly int mRadius;

        public Campfire(Texture2D texture, float timeToLive, Point position, bool isDead, int radius, Grid grid) : 
            base(texture, position.X, position.Y, grid)
        {
            mTimeToLive = timeToLive;
            mIsDead = isDead;
            mRadius = radius;
            mTextureRegion = new Rectangle(32, 0, 32, 32);
        }

        public Campfire(Texture2D texture, CampfireModel model, bool isDead, int radius, Grid grid) :
            base(texture, model.X, model.Y, grid)
        {
            mTimeToLive = model.TimeToLive;
            mIsDead = isDead;
            mRadius = radius;
            mTextureRegion = new Rectangle(32, 0, 32, 32);
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            return new CampfireModel()
            {
                TimeToLive = mTimeToLive,
                X = GetGridPos().X,
                Y = GetGridPos().Y
            };
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            mTimeToLive -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimeToLive < 0)
            {
                mIsDead = true;
                mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)

        {
            CreateCircleText(spriteBatch, mRadius);
            //var posittionRelativeToCamera = mGrid.PositionToCoordinatesRelativeToCamera(mPosition);
            //var positionTopLeftCornerOfRectangle = new Vector2( posittionRelativeToCamera.X - mRadius * mFieldSize, 
                //posittionRelativeToCamera.Y - mRadius * mFieldSize);
            //var diameter = 2 * mRadius + 1;
            //var rectangle = new Rectangle((int)positionTopLeftCornerOfRectangle.X, (int)positionTopLeftCornerOfRectangle.Y, diameter * mFieldSize, diameter * mFieldSize);
            if (!mIsDead)
            {
                base.Draw(spriteBatch, camera);
                //spriteBatch.Draw(mTexture, rectangle, Color.Red);
            }

        }

        private void CreateCircleText(SpriteBatch spriteBatch, int radius)
        {
            Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            //Debug.WriteLine(diam);
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
        }
    }
}
