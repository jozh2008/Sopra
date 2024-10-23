using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Input;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    public abstract class AbstractGameObjectGuiButton
    {
        protected readonly AbstractGameObject mAbstractGameObject;
        protected readonly SpriteFont mFont;
        private readonly InputHandler mInputHandler;
        private readonly Point mRelativePosition;
        private readonly Point mSize;
        protected string mText;
        private readonly Texture2D mTexture;
        private readonly GameState mGameState;

        private readonly Point mPos;
        private readonly int mMaxPosY = 144;



        protected AbstractGameObjectGuiButton(GameState gameState,
            AbstractGameObject abstractGameObject,
            Point size,
            Point relativePos,
            string text,
            SpriteFont spriteFont,
            Texture2D texture)
        {
            mAbstractGameObject = abstractGameObject;
            mText = text;
            mFont = spriteFont;
            mTexture = texture;
            mSize = size;
            mRelativePosition = relativePos;
            mInputHandler = gameState.mInputHandler;
            mGameState = gameState;

            mPos = new Point(mAbstractGameObject.GetPos().X, Math.Min(mMaxPosY, mAbstractGameObject.GetPos().Y));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var color = Color.White;

            var rectangle = new Rectangle(mPos.X + 5  + mRelativePosition.X,
                mPos.Y + mRelativePosition.Y,
                mSize.X,
                mSize.Y);
            var mouseRectangle = new Rectangle((int) mInputHandler.MousePosition.X,
                (int) mInputHandler.MousePosition.Y,
                1,
                1);
            if (mouseRectangle.Intersects(rectangle))
            {
                if (IsMainSchlittenMenu() || IsAbleToBuildEquipment(mGameState) || IsAbleToEquipHuman() || IsAbleToBuildSledgeStation(mGameState) || IsAbleToBuildDampfmaschine(mGameState))
                {
                    color = Color.Green;
                }
                else
                {
                    color = Color.Red;
                }
            }

            spriteBatch.Draw(mTexture, rectangle, color);
            if (!string.IsNullOrEmpty(mText))
            {
                var x = rectangle.X + rectangle.Width / 2 - mFont.MeasureString(mText).X / 2;
                var y = rectangle.Y + 5;
                if (!IsMainSchlittenMenu() && mText != "Kochen/Essen" && mText != "Kamin aktivieren/deaktivieren")
                {
                    spriteBatch.DrawString(mFont, mText, new Vector2(x, y), color);
                }
            }
        }

        //for color Green can build red not
        private bool IsMainSchlittenMenu()
        {
            if((mText == "Neues Schlittensegment"
                || mText == "Lager "
                || mText == "Objekt herstellen" 
                || (mText == "Kamin aktivieren/deaktivieren")
                || (mText == "Kochen/Essen")))
            {
                return true;
            }
            return false;
        }

        internal void HitboxCheck(GameState gameState, InputHandler inputHandler)
        {

            var rectangle = new Rectangle(mPos.X + 5 + mRelativePosition.X,

                mPos.Y + mRelativePosition.Y,

                mSize.X,

                mSize.Y);


            if (inputHandler.Inputs.IsSelected(rectangle) && IsMainSchlittenMenu())

            {

                Click(gameState);

                InputHandler.InputList.BlockFurtherInput();

            }

            else if (inputHandler.Inputs.IsSelected(rectangle))

            {

                ClickNewSledgeStation(gameState, mText);

                ClickObjektHerstellen(gameState, mText);

                mText = ClickLager(gameState, mText);

                InputHandler.InputList.BlockFurtherInput();

            }
        }

        //for color Green can build red not
        private bool IsAbleToBuildEquipment(GameState gameState)
        {
            if((gameState.AbleToBuildHolzaxt(false) && mText =="Holzaxt")||
                (gameState.AbleToBuildHolzbogen(false) && mText == "Holzbogen") ||
                (gameState.AbleToBuildHolzschwert(false) && mText == "Holzschwert") ||
                (gameState.AbleToBuildMetallaxt(false) && mText == "Metallaxt") ||
                (gameState.AbleToBuildMetallbogen(false) && mText == "Metallbogen")||
                (gameState.AbleToBuildMetallruestung(false) && mText == "Metallrüstung") ||
                (gameState.AbleToBuildMetallschwert(false) && mText == "Metallschwert"))
            {
                return true;
            }
            return false;
        }

        //for color Green can build red not
        private bool IsAbleToBuildSledgeStation(GameState gameState)
        {
            if(((((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Werkbank") ||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Küche")||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Lager") ||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Unterkunft") ||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Schmiede") ||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Hospiz")||
                (((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState) && mText == "Kamin")))
            {
                return true;
            }
            return false;
        }
        private bool IsAbleToBuildDampfmaschine(GameState gameState)
        {
            if(((Sledge)mAbstractGameObject).CanBuildDampfmaschine(gameState) && mText == "Dampf\n-maschine")
            {
                return true;
            }
            return false;
        }
        //for color Green can build red not
        private bool IsAbleToEquipHuman()
        {
            var gameObject =mGameState.mGrid.GetAbstractGameObjectAt(mGameState.mHumanPosition);
            if ((gameObject is Gatherers&& mText == "Holzaxt: " + mGameState.mEquipment["Holzaxt"] && mGameState.mEquipment["Holzaxt"]>=1) ||
                (gameObject is Archer && mText == "Holzbogen: " + mGameState.mEquipment["Holzbogen"] && mGameState.mEquipment["Holzbogen"] >= 1) ||
                (gameObject is Fighter && mText == "Holzschwert: " + mGameState.mEquipment["Holzschwert"] && mGameState.mEquipment["Holzschwert"] >= 1) ||
                (gameObject is Fighter && mText == "Metallschwert: "+ mGameState.mEquipment["Metallschwert"] && mGameState.mEquipment["Metallschwert"] >= 1) ||
                (gameObject is Archer && mText == "Metallbogen: " + mGameState.mEquipment["Metallbogen"] && mGameState.mEquipment["Metallbogen"] >= 1) ||
                (gameObject is Human && mText == "Metallrüstung: "+ mGameState.mEquipment["Metallrüstung"] && mGameState.mEquipment["Metallrüstung"]>=1) ||
                (gameObject is Gatherers && mText == "Metallaxt: " + mGameState.mEquipment["Metallaxt"] && mGameState.mEquipment["Metallaxt"] >= 1))
            {
                return true;
            }
            return false;
        }

        protected void BuildHolzaxt(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Wood, GameState.QuantityOfWoodNeededForHolzaxt);

        }

        protected void BuildHolzschwert(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Wood, GameState.QuantityOfWoodNeededForHolzschwert);

        }

        protected void BuildHolzbogen(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Wood, GameState.QuantityOfWoodNeededForHolzbogen);

        }

        protected void BuildMetallaxt(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Metall, GameState.QuantityOfMetallNeededForMetallaxt);
            gameState.mEquipment["Holzaxt"] -= 1;

        }

        protected void BuildMetallschwert(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Metall, GameState.QuantityOfMetallNeededForMetallschwert);
            gameState.mEquipment["Holzschwert"] -= 1;

        }

        protected void BuildMetallbogen(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Metall, GameState.QuantityOfMetallNeededForMetallbogen);

            gameState.mEquipment["Holzbogen"] -= 1;
        }

        protected void BuildMetallruestung(GameState gameState)
        {
            gameState.mResources.Decrease(ResourceType.Metall, GameState.QuantityOfMetallNeededForMetallruestung);

        }

        protected abstract void Click(GameState gameState);

        protected abstract void ClickNewSledgeStation(GameState gameState, string sledgeStation);
        protected abstract void ClickObjektHerstellen(GameState gameState, string sledgeStation);
        protected abstract string ClickLager(GameState gameState, string sledgeStation);
    }
}