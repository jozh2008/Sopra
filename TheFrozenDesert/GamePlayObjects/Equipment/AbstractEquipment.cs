using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheFrozenDesert.GamePlayObjects.Equipment
{
    public class AbstractEquipment
    {
        public enum EquipmentSlotType
        {
            Tool,
            Armor
        }
        public enum ItemType
        {
            Holzaxt,
            Metallaxt,
            Holzbogen,
            Metallbogen,
            Holzschwert,
            Metallschwert,
            Ruestung  
        }

        private readonly Rectangle mTextureRegion;
        private readonly Texture2D mTexture;

        public AbstractEquipment(Texture2D texture, ItemType iTemType)
        {
            mTexture = texture;
            switch (iTemType)
            {
                case ItemType.Holzaxt:
                    mTextureRegion = new Rectangle(32, 64, 32, 32);
                    break;
                case ItemType.Metallaxt:
                    mTextureRegion = new Rectangle(0, 64, 32, 32);
                    break;
                case ItemType.Holzbogen:
                    mTextureRegion = new Rectangle(32, 32, 32, 32);
                    break;
                case ItemType.Metallbogen:
                    mTextureRegion = new Rectangle(0, 32, 32, 32);
                    break;
                case ItemType.Holzschwert:
                    mTextureRegion = new Rectangle(32, 0, 32, 32);
                    break;
                case ItemType.Metallschwert:
                    mTextureRegion = new Rectangle(0, 0, 32, 32);
                    break;
                case ItemType.Ruestung:
                    mTextureRegion = new Rectangle(0, 96, 32, 32);
                    break;
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Point position)
        {
            var rect = new Rectangle(position.X, position.Y, 32, 32);
            spriteBatch.Draw(mTexture, rect, mTextureRegion, Color.White);
        }
    }
}