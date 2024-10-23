using Microsoft.Xna.Framework;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    internal class SledgeGui : AbstractGameObjectGui
    {
        public SledgeGui(GameState gameState, Sledge sledge) :
            base(sledge,
                gameState.mGameObjectGuiTexture,
                new Point(gameState.mGameObjectGuiTexture.Width,
                    gameState.mGameObjectGuiTexture.Height-128),
                "Sledge",
                gameState.mSpriteFontMenu)
        {
            mMaxY = 144;
            mButtons.Add(new AddNewSledgeButton(gameState,
                
                sledge,
                gameState.mSpriteFontMenu,
                "Neues Schlittensegment",
                new Point(gameState.mGameObjectGuiTexture.Width - 10, 64),
                new Point(32, 32),
                gameState.mNeuesSchlittensegmentButtonTexture));
            
           mButtons.Add(new AddNewSledgeButton(gameState,
                sledge,
                gameState.mSpriteFontMenu,
                "Kochen/Essen",
                new Point(gameState.mGameObjectGuiTexture.Width - 10, 64),
                new Point(32, 96),
                 gameState.mKochenEssenButtonTexture));
            mButtons.Add(new AddNewSledgeButton(gameState,
                sledge,
                gameState.mSpriteFontMenu,
                "Kamin aktivieren/deaktivieren",
                new Point(gameState.mGameObjectGuiTexture.Width - 10, 64),
                new Point(32, 160),
                 gameState.mKaminButtonTexture));
            mButtons.Add(new AddNewSledgeButton(gameState,
                sledge,
                 gameState.mSpriteFontMenu,
                "Objekt herstellen",
                new Point(gameState.mGameObjectGuiTexture.Width - 10, 64),
                new Point(32, 224),
                 gameState.mObjektHerstellenButtonTexture));
            mButtons.Add(new AddNewSledgeButton(gameState,
                sledge,
                 gameState.mSpriteFontMenu,
                "Lager ",
                new Point(gameState.mGameObjectGuiTexture.Width - 10, 64),
                new Point(32, 288),
                 gameState.mLagerButtonTexture));
            
        }
    }
}