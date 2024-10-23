using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.AI;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class Arrow
    {
        private Vector2 mPosition;
        private readonly Vector2 mMovement;
        private float mTimeToLive;
        private readonly int mDamage;
        private readonly EFaction mFaction;
        private readonly Texture2D mTexture;
        //public bool mDestroy = false;
        public bool mDestroy;
        private const int FieldSize = 32;
        readonly float mAngle;

        public Arrow(Vector2 position, Vector2  movement, float firingAngle, float timeToLive, int damage, EFaction faction, Texture2D texture,bool destroy)
        {
            mPosition = position;
            mMovement = movement;
            mTimeToLive = timeToLive;
            mDamage = damage;
            mFaction = faction;
            mTexture = texture;
            mAngle = firingAngle;
            mDestroy = destroy;
        }

        public void Update(GameTime gameTime, Grid grid, GameState gameState)
        {
            mTimeToLive -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimeToLive < 0)
            {
                mDestroy = true;

            }
            if (!mDestroy)
            {
                mPosition += mMovement;
                HandleCollision(grid, gameState);
            }


        }

        private void HandleCollision(Grid grid, GameState gameState)
        {
            // for checking in the middle

            //Vector2 posiion2 = new Vector2(mPosition.X +mFieldSize/2, mPosition.Y + mFieldSize / 2);
            Vector2 posiion2 = new Vector2(mPosition.X, mPosition.Y );
            var collisionObject = grid.GetAbstractGameObjectAt(grid.PositionToGridCoordinates(posiion2));
            //Debug.WriteLine(mPosition);
            if (collisionObject is ResourceObject )
            {
                mDestroy = true;
            }

            if (collisionObject is Human)
            { 
                Human collisionHuman = (Human) collisionObject;
                if (collisionHuman.Faction != mFaction)// can't attack human from same faction
                {
                    if (collisionHuman.Faction == EFaction.Enemy)
                    {
                        collisionHuman.mRoutineHandler.SetRoutine(Routine.Attack);
                    }
                    if (!mDestroy)// arrow make damage if not destroyed
                    {
                        collisionHuman.DamageHuman(mDamage, gameState);
                        mDestroy = true;// after arrow hit from other faction, arrow gets destroyed
                    }
                }
            }

            if (collisionObject is Wolf)
            {
                Wolf collisionWolf = (Wolf) collisionObject;
                collisionWolf.Damage(mDamage, mFaction);
                mDestroy = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!mDestroy)//just draw if arrow is not destroy yet
            {
          
                var rect = new Rectangle((int)(mPosition.X  - camera.PositionPixels.X),
                    (int)(mPosition.Y  - camera.PositionPixels.Y),
                    FieldSize,
                    FieldSize);
                spriteBatch.Draw(mTexture, rect, null, Color.Red, MathHelper.ToRadians(mAngle), Vector2.Zero,SpriteEffects.None, 0f);
                //spriteBatch.Draw(mTexture, rect, Color.White);


            }
        }
        
       

    }
}
