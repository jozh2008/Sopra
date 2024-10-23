using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.AI;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.MainMenu.SubMenu;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;
using Rectangle = System.Drawing.Rectangle;

namespace TheFrozenDesert.GamePlayObjects
{
    public class GamePlayObjectFactory
    {
        private readonly Game1 mGame;
        private readonly GameState mGameState;
        private readonly Grid mGrid;
        public GamePlayObjectFactory(Game1 game, GameState gameState, Grid grid)
        {
            mGame = game;
            mGameState = gameState;
            mGrid = grid;
        }
        public void CreateArcher(int posX, int posY, EFaction faction, Routine routine = Routine.Nothing)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromFaction(faction, "GameplayObjects/ArcherSpritesheet"));
            var arrowTexture = mGame.GetContentManager().GetTexture("GameplayObjects/arrow");
            if (faction == EFaction.Enemy)
            {
                mGrid.AddToGrid(posX, posY, new Archer(texture, arrowTexture, posX, posY, 100, 3, 100, 100, mGrid, mGameState, faction, routine));
            }
            else
            {
                mGrid.AddToGrid(posX, posY, new Archer(texture, arrowTexture, posX, posY, 100, 10, 100, 100, mGrid, mGameState, faction, routine));
            }
        }

        public void CreateSuperArcher(int posX, int posY, EFaction faction, Routine routine = Routine.Nothing)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromFaction(faction, "GameplayObjects/ArcherSpritesheet"));
            var arrowTexture = mGame.GetContentManager().GetTexture("GameplayObjects/arrow");
            mGrid.AddToGrid(posX, posY, new Archer(texture, arrowTexture, posX, posY, 100, 10, 10000, 10000, mGrid, mGameState, faction, routine));
        }

        public void CreateFighter(int posX, int posY, EFaction faction, Routine routine = Routine.Nothing)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromFaction(faction, "GameplayObjects/SwordSpritesheet"));
            if (faction == EFaction.Enemy)
            {
                mGrid.AddToGrid(posX, posY, new Fighter(texture, posX, posY, 100, 3, 100, 100, mGrid, mGameState, faction, routine));
            }
            else
            {
                mGrid.AddToGrid(posX, posY, new Fighter(texture, posX, posY, 100, 10, 100, 100, mGrid, mGameState, faction, routine));
            }
        }

        public void CreateGatherer(int posX, int posY, EFaction faction, Routine routine = Routine.Nothing)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromFaction(faction, "GameplayObjects/AxeSpritesheet"));
            mGrid.AddToGrid(posX, posY, new Gatherers(texture, posX, posY, 100, 20, 100, 100, mGrid, mGameState, 3,1, faction, routine));
        }

        public void CreateGatherer(GathererModel model)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromModel(model, "GameplayObjects/AxeSpritesheet"));
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, new Gatherers(texture, mGrid, mGameState, model));
        }

        public void CreateArcher(ArcherModel model)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromModel(model, "GameplayObjects/ArcherSpritesheet"));
            var arrowTexture = mGame.GetContentManager().GetTexture("GameplayObjects/arrow");
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, new Archer(texture, arrowTexture, mGrid, mGameState, model));
        }

        public void CreateFighter(FighterModel model)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromModel(model, "GameplayObjects/SwordSpritesheet"));
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, new Fighter(texture, mGrid, mGameState, model));
        }

        private Generator CreateGenerator(int posX, int posY)
        {
            var generatorTexture = mGame.GetContentManager().GetTexture("GameplayObjects/ReaktorSpritesheet");
            Generator generator = new Generator(generatorTexture, posX, posY, mGrid);
            mGrid.AddToGrid(posX, posY, generator);
            return generator;
        }

        public Generator CreateReactor(int posX, int posY)
        {
            Generator generator = null;
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (i == 0 && j == 1)
                    {
                        generator = CreateGenerator(posX + i, posY + j);
                    }
                    else
                    {
                        var generatorTexture = mGame.GetContentManager().GetTexture("GameplayObjects/ReaktorSpritesheet");
                        ReactorPart part = new ReactorPart(generatorTexture, posX + i, posY + j, mGrid, new Point(i * 32, j * 32));
                        mGrid.AddToGrid(posX + i, posY + j, part);
                    }
                }
            }

            return generator;
        }

        public Sledge CreateSledge(int posX, int posY)
        {
            var texture = mGame.GetContentManager().GetTexture("GameplayObjects/Segments");
            var sledge = new Unterkunft(texture, posX, posY, 100, null, mGrid);
            mGrid.AddToGrid(posX, posY, sledge);
            sledge.TryAttachSledgeType(mGameState, mGrid,"Lager");
            sledge.TryAttachSledgeType(mGameState, mGrid, "Werkbank");
            return sledge;

        }

        private Wolf CreateWolf(int posX, int posY, WolfPack wolfPack)
        {
            var wolf = new Wolf(mGame.GetContentManager().GetTexture("GameplayObjects/Wolf_rechts"),
                posX,
                posY,
                115,
                mGrid, 
                100, 
                5, 
                2,
                10,
                5,
                wolfPack);
            mGrid.AddToGrid(posX,posY, wolf);
            return wolf;
        }
        public Wolf CreateWolf(WolfModel model)
        {
            var wolf = new Wolf(mGame.GetContentManager().GetTexture("GameplayObjects/Wolf_rechts"), model, 115, mGrid);
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, wolf);
            return wolf;
        }

        public void CreateWolfPack(int posX, int posY, int numberOfWolves)
        {
            // define Area for wolves as on field^2 per Wolf
            var area = new Rectangle(posX, posY, numberOfWolves, numberOfWolves);
            
            var wolfPack = new WolfPack(area);
            
            // add wolves to Grid
            for (int i = 0; i < numberOfWolves; i++)
            {
                wolfPack.Add(CreateWolf(posX + i, posY + i, wolfPack));
            }
            mGameState.mWolfPacks.Add(wolfPack);
        }

        public void CreateImportantWolfPack(int posX, int posY)
        {
            var area = new Rectangle(posX, posY, 5, 5);

            var wolfPack = new WolfPack(area);

            CreateKey(posX + 2, posY + 2);
            // add wolves in Rectangle around key
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    wolfPack.Add(CreateWolf(posX + x, posY + y, wolfPack));
                }
            }

        }

        public void CreateSign(int posX, int posY, String message)
        {
            var sign = new Sign(mGame.GetContentManager().GetTexture("GameplayObjects/accessories"),
                mGame.GetContentManager().GetTexture("GameplayObjects/sign_gui"),
                mGame.GetContentManager().GetFont(),
                posX,
                posY,
                mGrid,
                message);
            mGrid.AddToGrid(posX, posY, sign);
        }

        public void CreateBridge(int posX, int posY, int length)
        {
            for (int i = 0; i < length; i++)
            {
                CreateBridge(posX+i, posY);
            }
        }

        public void CreateCampfire(int posX, int posY, int campfireRadius)
        {
            var gridPositionCampfire = new Point(posX, posY);
            var campfire = new Campfire(mGame.GetContentManager().GetTexture("GameplayObjects/KeyFire"), 20, gridPositionCampfire, false, campfireRadius, mGrid);
            
            mGrid.AddToGrid(posX,posY,campfire);
        }

        public void CreateCampfire(CampfireModel model)
        {
            var campfire = new Campfire(mGame.GetContentManager().GetTexture("GameplayObjects/KeyFire"), model, false, 1, mGrid);
            mGrid.AddToGrid(model.X, model.Y, campfire);
        }

        public Key CreateKey(int posX, int posY)
        {
            var key = new Key(mGame.GetContentManager().GetTexture("GameplayObjects/KeyFire"), posX, posY, mGrid);
            
            mGrid.AddToGrid(posX,posY,key);
            return key;
        }

        public void CreateKey(KeyModel model)
        {
            var key = new Key(mGame.GetContentManager().GetTexture("GameplayObjects/KeyFire"), model.GridCurrentPositionX, model.GridCurrentPositionY, mGrid);

            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, key);
        }

        public void CreateBridge(BridgeModel model)
        {
            var bridge = new Bridge(mGame.GetContentManager().GetTexture("GameplayObjects/bridgebroken"),
                mGame.GetContentManager().GetTexture("GameplayObjects/bridgerepaired"),
                model,
                mGrid);
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, bridge);
            if (model.mRepairState >= 1)
            {
                bridge.SetRepaired();
            }
        }

        private void CreateSnowTile(int posX, int posY, bool containsKey)
        {
            var snowTile = new SnowTile(mGame.GetContentManager().GetTexture("GameplayObjects/SnowTile"),
                posX,
                posY,
                mGrid,
                containsKey,
                1);
            mGrid.AddToGrid(posX, posY, snowTile);
        }

        public void CreateSnowTile(SnowTileModel model)
        {
            var snowTile = new SnowTile(mGame.GetContentManager().GetTexture("GameplayObjects/SnowTile"),
                model,
                mGrid,
                1);
            mGrid.AddToGrid(model.GridCurrentPositionX, model.GridCurrentPositionY, snowTile);
        }

        public void CreateBigTree(int posX, int posY, String message)
        {
            var tree = new BigTree(mGame.GetContentManager().GetTexture("GameplayObjects/BigTree"),
                posX,
                posY,
                mGrid,
                mGameState,
                0.1,
                message);
            
            mGrid.AddToGrid(posX, posY, tree);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    mGrid.AddToGrid(posX + x, posY + y, new BigTreePart(mGame.GetContentManager().GetBlankTexture(), posX + x, posY + y, mGrid, tree));
                }
            }
        }

        public void CreateAvalange(Rectangle pos)
        {
            Random r = new Random();
            var randX = r.Next(0,pos.Width) + pos.X;
            var randY = r.Next(0, pos.Height) + pos.Y;
            
            for (int x = pos.X; x < pos.Right; x++)
            {
                for (int y = pos.Y; y < pos.Bottom; y++)
                {
                    if (x == randX && y == randY)
                    {
                        CreateSnowTile(x, y, true);
                    }
                    else
                    {
                        CreateSnowTile(x, y, false);
                    }
                }
            }
        }

        private void CreateBridge(int posX, int posY)
        {
            var bridge = new Bridge(mGame.GetContentManager().GetTexture("GameplayObjects/bridgebroken"),
                mGame.GetContentManager().GetTexture("GameplayObjects/bridgerepaired"), 
                posX,
                posY,
                mGrid);
            mGrid.AddToGrid(posX, posY, bridge);
            //return bridge;
        }

        public void CreateBlockTile(int posX, int posY)
        {
            var gameObject = mGrid.GetAbstractGameObjectAtNewGridPosition(new Vector2(posX, posY));
            if (gameObject is EmptyObject emptyObject)
            {
                emptyObject.Block();
            }
            
        }

        public Radio CreateRadio()
        {
            var radio = new Radio(mGame.GetContentManager().GetTexture("GameplayObjects/sign_gui"),
                mGame.GetContentManager().GetFont(), "Laufe von hier aus nach unten um die Tanne des ewigen Winters zu finden.");
            return radio;
        }
        // tools
        public Axe CreateAxe(Axe.AxeType type)
        {
            return new Axe(type, false);

        }

        public Sword CreateSword(Sword.SwordType type)
        {
            return new Sword(type, false);
        }

        public Bow CreateBow(Bow.BowType type)
        {
            return new Bow(type, false);
        }

        private string GetTextureNameFromModel(HumanModel model, string name)
        {
            switch (model.Faction)
            {
                case "Player":
                    return name;
                case "Neutral":
                    return name + "Neutral";
                case "Enemy":
                    return name + "Enemy";
            }
            return name;
        }

        private string GetTextureNameFromFaction(EFaction faction, string name)
        {
            switch (faction)
            {
                case EFaction.Player:
                    return name;
                case EFaction.Neutral:
                    return name + "Neutral";
                case EFaction.Enemy:
                    return name + "Enemy";
            }
            return name;
        }


        public Fighter CreateOmegaFighter(int posX, int posY, EFaction faction, Routine routine = Routine.Nothing)
        {
            Texture2D texture = mGame.GetContentManager().GetTexture(GetTextureNameFromFaction(faction, "GameplayObjects/SwordSpritesheet"));
            var fighter = new Fighter(texture,
                posX,
                posY,
                100,
                10000,
                10000,
                10000,
                mGrid,
                mGameState,
                faction,
                routine);
            mGrid.AddToGrid(posX, posY, fighter );

            return fighter;
        }
    }
}
