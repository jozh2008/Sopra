using Microsoft.Xna.Framework;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    internal class ResourceObjectGui : AbstractGameObjectGui
    {
        public ResourceObjectGui(GameState gameState, ResourceObject gameObject) :
            base(gameObject,
                gameState.mGameObjectGuiTexture,
                new Point(gameState.mGameObjectGuiTexture.Width,
                    gameState.mGameObjectGuiTexture.Height),
                gameObject.ReturnResourceObjectTypeAsString(),
                gameState.mSpriteFont)
        {
        }
    }
}