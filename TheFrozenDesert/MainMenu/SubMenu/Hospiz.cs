using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.MainMenu.SubMenu
{
    internal sealed class Hospiz: Sledge
    {
        private readonly int mRange = 2;

        public Hospiz(Texture2D texture, int gridPosX, int gridPosY, float movementSpeed, Sledge nextSledge, Grid grid) : base(texture, gridPosX, gridPosY, movementSpeed, nextSledge, grid)
        {
            mTextureRegion = new Rectangle(0, 32, 32, 32);
        }

        public Hospiz(HospizModel model,
            GameState gameState,
            Texture2D texture2D,
            float movementSoeed,
            Grid grid) :
            base(gameState, texture2D, model, movementSoeed, grid)
        {
            mTextureRegion = new Rectangle(0, 32, 32, 32);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            base.Update(gameTime, grid, gameState, soundManager);
            IsinsideHospizRange(this.GetGridPos(), gameState, mRange,gameTime);
        }

        public override AbstractGameObjectModel Serialize(GameState gameState)
        {
            var sledgeModel = new HospizModel()
            {
                X = mGridPos.X + gameState.GetGridOffset().X,
                Y = mGridPos.Y + gameState.GetGridOffset().Y,
                PreviousSledgeUuid = mPreviousSledge is null ? "none" : mPreviousSledge.Uuid.ToString(),
                Uuid = Uuid.ToString()
            };
            return sledgeModel;
        }

        private void IsinsideHospizRange(Point sledge, GameState gameState, int range,GameTime gameTime)
        {
            var gameObjectSledge = gameState.mGrid.GetAbstractGameObjectAt(sledge);
            if (gameObjectSledge is Hospiz)
            {
                for (var i = -range; i <= range; i++)
                {
                    for (var j = -range; j <= range; j++)
                    {
                        Point human = new Point(sledge.X + i, sledge.Y + j);
                        if (human.X >= 0 && human.Y >= 0)
                        {
                            var gameObject = gameState.mGrid.GetAbstractGameObjectAt(human);
                           
                            if (gameObject is Human {Faction: EFaction.Player} humanObject)
                            {
                                //Debug.WriteLine(humanObject.Health + " " + humanObject.GetGridPos());
                                humanObject.AddHealth(5, gameTime);
                            }

                        }
                    }
                }
            }
        }
        /*
        public void AddAllFlesh(int fleshs)
        {
            for (int i = 0; i < fleshs; i++)
            {
                AddRawFood(5);
            }
        }

        /*
        public void AddRawFood(int healthHealth)
        {
            health.Add(healthHealth);
        }

        public void AddCookedFood(int healthHealth, int healthMultiplier)
        {
            health.Add(healthHealth * healthMultiplier);
        }

        public void Health()
        {

            //human.AddHealth(15);

        }
        */

    }
}
