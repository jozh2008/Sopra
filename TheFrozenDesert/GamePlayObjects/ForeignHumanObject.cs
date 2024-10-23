using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects
{
    public class ForeignHumanObject : Human
    {
        public ForeignHumanObject(Texture2D texture,
            int posX,
            int posY,
            float speed,
            int attack,
            int health,
            int heat,
            Grid grid,
            GameState gameState,
            EFaction faction) :
            base(texture, posX, posY, speed, attack, health, heat, grid, gameState, faction)
        {
        }

        public override bool MissingEquipment()
        {
            return false;
        }

        internal override void TryToCraftToolEnemy()
        {
            
        }
    }
}