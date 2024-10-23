using System;
using Microsoft.Xna.Framework;
using TheFrozenDesert.GamePlayObjects;

namespace TheFrozenDesert.AI
{
    public class DangerLevel
    {
        private float mCurrentDanger;
        private Point mSledgeReferencePosition;
        private readonly Sledge mPlayerSledge;
        private readonly float mDangerResetDistance;
        private readonly float mDangerIncreaseSpeed;
        public DangerLevel(Sledge playerSledge, float dangerResetDistance = 10, float dangerIncreaseSpeed = 0.01f, float initialDanger = 0)
        {
            mPlayerSledge = playerSledge;
            mSledgeReferencePosition = mPlayerSledge.GetGridPos();
            mDangerResetDistance = dangerResetDistance;
            mDangerIncreaseSpeed = dangerIncreaseSpeed;
            mCurrentDanger = initialDanger;
        }

        public void Update(GameTime gameTime)
        {
            if ((mPlayerSledge.GetGridPos() - mSledgeReferencePosition).ToVector2().Length() > mDangerResetDistance)
            {
                mCurrentDanger = 0;
                mSledgeReferencePosition = mPlayerSledge.GetGridPos();
            }
            else
            {
                mCurrentDanger += mDangerIncreaseSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }

            //System.Diagnostics.Debug.WriteLine(GetNormalizedDanger());
        }


        // Normalize danger level, with adjusted sigmoid function.
        public float GetNormalizedDanger()
        {
            return (float) (1 / (1 + Math.Pow(Math.E, (-mCurrentDanger + 15) * 0.5)));
        }
    }
}
