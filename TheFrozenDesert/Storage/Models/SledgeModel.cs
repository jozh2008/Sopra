using TheFrozenDesert.Content;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.MainMenu.SubMenu;
using TheFrozenDesert.States;

namespace TheFrozenDesert.Storage.Models
{
    public class SledgeModel : AbstractGameObjectModel
    {
        public string PreviousSledgeUuid { get; set; }
        public string Uuid { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Sledge CreateSledge(GameState gameState, FrozenDesertContentManager manager)
        {
            return this switch
            {
                PlainSledgeModel _ => new Sledge(gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    this,
                    100,
                    gameState.GetCurrentGrid()),
                HospizModel _ => new Hospiz((HospizModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                KitchenModel _ => new Kitchen((KitchenModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                SteamEngineModel _ => new SteamEngine((SteamEngineModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                CraftingSledgeModel _ => new Workbench((CraftingSledgeModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                StockModel _ => new Stock((StockModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                ForgeModel _ => new Forge((ForgeModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                OvenModel _ => new Kamin((OvenModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                HousingModel _ => new Unterkunft((HousingModel) this,
                    gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    100,
                    gameState.GetCurrentGrid()),
                _ => new Sledge(gameState,
                    manager.GetTexture("GameplayObjects/Segments"),
                    this,
                    100,
                    gameState.GetCurrentGrid())
            };
        }
    }
    public sealed class PlainSledgeModel : SledgeModel
    {}
    public sealed class HospizModel : SledgeModel
    {}
    public sealed class KitchenModel : SledgeModel
    {}
    public sealed class SteamEngineModel : SledgeModel
    {}
    public sealed class CraftingSledgeModel : SledgeModel
    {}
    public sealed class StockModel : SledgeModel
    {}
    public sealed class ForgeModel : SledgeModel
    {}
    public sealed class OvenModel : SledgeModel
    {}
    public sealed class HousingModel : SledgeModel
    {}
}