using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using TheFrozenDesert.AI;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.GenerateMap;
using TheFrozenDesert.Input;
using TheFrozenDesert.Pathfinding;
using TheFrozenDesert.Storage.Models;
using Rectangle = System.Drawing.Rectangle;

namespace TheFrozenDesert.States
{
    public sealed class GameState : State
    {
        private readonly Camera mCamera;
        public readonly Texture2D mGameObjectGuiTexture;
        public readonly Texture2D mTreeRockSpriteSheet;
        public readonly Texture2D mWeapontexture;
        public readonly int mGameStateId;
        private readonly Generator mGenerator;

        public GamePlayObjectFactory GamePlayObjectFactory {get; }

        //A grid of visible Spaces during gameplay
        public readonly Grid mGrid;
        public readonly Texture2D mGuiItemSlotTexture;

        private readonly AiPlayer mAiPlayer;

        //Textures
        public readonly Texture2D mLagerButtonTexture;
        public readonly Texture2D mNeuesSchlittensegmentButtonTexture;
        public readonly Texture2D mKaminButtonTexture;
        public readonly Texture2D mObjektHerstellenButtonTexture;
        public readonly Texture2D mKochenEssenButtonTexture;
        public readonly Texture2D mMissingTexture;
        public readonly PathFinder mPathFinder;
        private readonly ResourceCounter mResourceCounter;
        private readonly NotificationField mNotificationField;
        

        public readonly Resources mResources;
        public readonly Resources mEnemyResources;
        internal readonly Dictionary<Guid, Human> mPlayerAliveHumans;
        public Dictionary<string, int> mEquipment;

        internal List<AbstractGameObjectGuiButton> mButtonsAbstractGameObjectGui;
        internal List<WolfPack> mWolfPacks = new List<WolfPack>();

        //Spritefont
        public readonly SpriteFont mSpriteFont;
        public readonly SpriteFont mSpriteFontMenu;
        public readonly SpriteFont mSpriteFontNotification;

        private readonly TiledMapRenderer mTiledMapRenderer;
        public AbstractGameObject mGuiOpen;
        public int mKillCounter;
        public int mKeyCounter;
        private int mKillsAfterKeys;

        public Radio mRadio;

        public bool mHasDampfmaschine;
        public bool mKaminIsActivated;
        public bool mHasKamin;
        public bool mHasSchmiede;
        public bool mHasKueche;

        public string mHasEnoughWoodForMovingSledge = "";
        public string mHasEnoughResourceForBuildingSledgeStation = "";
        public string mHasEnoughResourceForBuildingItems = "";
        public string mHasEnoughResourceForCookingFood = "";
        public string mNotificationForRecruting = "";

        public const int QuantityOfWoodNeededForHolzaxt = 2;
        public const int QuantityOfMetallNeededForMetallaxt = 3;
        public const int QuantityOfWoodNeededForHolzschwert = 3;
        public const int QuantityOfMetallNeededForMetallschwert = 3;
        public const int QuantityOfWoodNeededForHolzbogen = 4;
        public const int QuantityOfMetallNeededForMetallbogen = 7;
        public const int QuantityOfMetallNeededForMetallruestung = 7;
        private const int GridSizeX = 575;
        private const int GridSizeY = 177;
        public const int QuantityHolzaxtForMetallaxt = 1;
        public const int QuantityHolzschwertForMetallschwert = 1;
        public const int QuantityHolzbogenForMetallbogen = 1;
        private double Timer { get; set; }

        public int mCapacityHumanInSledge = 5;


        public Sledge Sledge { get; set; }

        public Point mHumanPosition;

        public Game1.Managers mManagers;

        private int mDrawCalls;
        private double mTimeUntilNextFramesUpdate = 1;
        public int CurrentFps { get; private set; }

        public GameState(Game1 game,
            GraphicsDevice graphicsDevice,
            ContentManager content,
            InputHandler input,
            GameWindow window,
            int gameStateId, Boolean isTechDemo) : base(game, graphicsDevice, content, input)
        {
            mManagers = game.mManagers;
            mCamera = new Camera(graphicsDevice, window);
            mPathFinder = new PathFinder(Game1.MapWidth, Game1.MapHeight);
            mMissingTexture = game.GetContentManager().GetTexture("GameplayObjects/missingTexture");
            game.GetContentManager().GetTexture("GameplayObjects/testObject");
            mSpriteFont = mContent.Load<SpriteFont>("Fonts/Fonts");
            mSpriteFontMenu = mContent.Load<SpriteFont>("Fonts/Font");
            mSpriteFontNotification = mContent.Load<SpriteFont>("Fonts/FontsNotification");
            var tiledMap = mContent.Load<TiledMap>("Map/FrozenDesertMap");
            mTiledMapRenderer = new TiledMapRenderer(graphicsDevice, tiledMap);
            mGrid = new Grid(this, mCamera);
            mGameObjectGuiTexture = game.GetContentManager().GetTexture("GUI/GameObjectGUIBg");
            mGuiItemSlotTexture = game.GetContentManager().GetTexture("GUI/EmptySlot");
            game.GetContentManager().GetTexture("GameplayObjects/BetaGameplayObjectSpriteSheet");
            mTreeRockSpriteSheet = game.GetContentManager().GetTexture("GameplayObjects/TreeRockSpriteSheet");
            mGameStateId = gameStateId;
            InitGrid(0, 0, GridSizeX, GridSizeY);
            GamePlayObjectFactory = new GamePlayObjectFactory(game, this, mGrid);
            mWeapontexture = game.GetContentManager().GetTexture("GameplayObjects/Icons");
            mPlayerAliveHumans = new Dictionary<Guid, Human>();
            mLagerButtonTexture = game.GetContentManager().GetTexture("GUI/lager");
            mKochenEssenButtonTexture= game.GetContentManager().GetTexture("GUI/kochenessen");
            mKaminButtonTexture = game.GetContentManager().GetTexture("GUI/kamin");
            mObjektHerstellenButtonTexture = game.GetContentManager().GetTexture("GUI/objektherstellen");
            mNeuesSchlittensegmentButtonTexture = game.GetContentManager().GetTexture("GUI/neues schlittensegment"); 
            // Generate map Objects to Grid
            var mapGenerator = new MapGenerator(tiledMap, mGrid, this, GamePlayObjectFactory);
            mapGenerator.PlaceBlockFields();
            mResources = new Resources();
            mEnemyResources = new Resources();
            mEquipment = new Dictionary<string, int>
            {
                { "Holzaxt",0 },
                { "Metallaxt",0 },
                { "Holzschwert",0 },
                { "Metallschwert",0 },
                { "Holzbogen",0 },
                { "Metallbogen",0 },
                { "Metallrüstung",0 }

            };
            mHasDampfmaschine = false;
            mAiPlayer = new AiPlayer(GamePlayObjectFactory,  5, mEnemyResources, this, isTechDemo);
            var safeExists = LoadGameState();
            mResourceCounter = new ResourceCounter(mSpriteFont);
            mNotificationField = new NotificationField(mSpriteFont);
            mButtonsAbstractGameObjectGui = new List<AbstractGameObjectGuiButton>();
            mGenerator = GamePlayObjectFactory.CreateReactor(487, 87);
            GamePlayObjectFactory.CreateSign(6,
                4,
                "Um Eine Neutrale figur zu rekrutieren: Stelle dich neben sie und drücke R");

            // Schlitten erklärung
            GamePlayObjectFactory.CreateSign(3,
                11,
                "Um Dein Schlitten zu benutzen, muss sich eine Einheit direkt daneben befinden.");
            GamePlayObjectFactory.CreateSign(2,
                11,
                "Im Schlittenmenü kannst du neue Werkzeuge bauen und deine Einheiten ausrüsten.");

            // erste Mission Dampfmaschine
            GamePlayObjectFactory.CreateSign(8,
                10,
                "Sammle Resourcen um deinen Schlitten Fortbewegungsfähig zu machen (Dampfmaschine)");

            // Hinweis zur Lawine
            GamePlayObjectFactory.CreateSign(60,
                48,
                "Man flüstert Nordwestlich von hier versteckt sich ein Schlüssel unter einer Lawine");
            if (!safeExists)
            {
                mKaminIsActivated = false;
                mHasKamin = false;
                mHasKueche = false;
                mHasSchmiede = false;
                if (!isTechDemo)
                {
                    GamePlayObjectFactory.CreateGatherer(3, 9, EFaction.Player);
                    GamePlayObjectFactory.CreateGatherer(3, 8, EFaction.Player);
                    GamePlayObjectFactory.CreateFighter(2, 9, EFaction.Player);
                    GamePlayObjectFactory.CreateBridge(42, 48, 14);
                    GamePlayObjectFactory.CreateKey(58, 48);    // Schlüssel hinter der Brücke
                    
                    // Schild erklärung Rekrutieren
                    GamePlayObjectFactory.CreateArcher(7,4, EFaction.Neutral);
                    GamePlayObjectFactory.CreateBigTree(1, 31, "Um Die Welt aufzutauen, musst du einen Reaktor im Westen aktivieren (Sammle alle Schlüssel)");
                    GamePlayObjectFactory.CreateAvalange(new Rectangle(71, 30, 8, 9));
                }
                var sledge = GamePlayObjectFactory.CreateSledge(2, 10);
                mCamera.ActivateCenterObjekt(sledge);
                Sledge = sledge;

                // Creating Gameplay objects
                if (!isTechDemo)
                {
                    mapGenerator.GenerateImportantWolfPack();
                    mapGenerator.GenerateWolfPacks(70);
                    mapGenerator.GenerateObjects(new List<ResourceObject.ResourceObjectType> { ResourceObject.ResourceObjectType.Tree }, 4, 0.05);
                    mapGenerator.GenerateObjects(new List<ResourceObject.ResourceObjectType> { ResourceObject.ResourceObjectType.Rock }, 5, 0.2);
                    mapGenerator.GenerateNeutralPlayers(200);

                    GamePlayObjectFactory.CreateWolfPack(15, 14, 4);
                }
            }
            mAiPlayer.SetSledge(Sledge);
            SaveGameState();
            game.GetSoundManager().GameSound();
            // for testing wolf
        }

        public void AddEnemyHuman(Human human)
        {
            mAiPlayer.AddHuman(human);
        }

        public void MakeTechDemo()
        {
            var omegaFighter = GamePlayObjectFactory.CreateOmegaFighter(25, 20, EFaction.Player);
            mCamera.ActivateCenterObjekt(omegaFighter);
            mAiPlayer.Enable();
            int unitsPlaced = 0;
            for (int i = 0; i < mGrid.Size.X; i += 2)
            {
                for (int j = 0; j < mGrid.Size.Y; j += 2)
                {
                    if (unitsPlaced > 1000)
                    {
                        return;
                    }

                    if (mGrid.GetAbstractGameObjectAtNewGridPosition(new Vector2(i, j)) is EmptyObject)
                    {
                        GamePlayObjectFactory.CreateSuperArcher(i, j, EFaction.Enemy);
                        //GamePlayObjectFactory.CreateFighter(j, i, EFaction.Enemy);
                        unitsPlaced++;
                    }
                }
            }
        }

        internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mGraphicsDevice.Clear(Color.Black);
            spriteBatch.End();
            mTiledMapRenderer.Draw(mCamera.GetView);
            spriteBatch.Begin();
            mGrid.Draw(gameTime, spriteBatch);
            
            mGuiOpen?.DrawGui(gameTime, spriteBatch,this);
            mRadio?.Draw(gameTime, spriteBatch, this);
            
            mResourceCounter.Draw(gameTime, spriteBatch, this);
            mNotificationField.Draw(gameTime, spriteBatch, this);
            

            mGame.GetSelectionManager().Draw(gameTime, spriteBatch);
            mDrawCalls++;
        }

        internal override void Update(GameTime gameTime, Game1.Managers managers)
        {
            mAiPlayer.Update(gameTime, mCamera, this);
            // campfire
            Timer += gameTime.ElapsedGameTime.TotalSeconds;
            mRadio = mRadio?.Update(gameTime, managers);
            mTimeUntilNextFramesUpdate -= gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimeUntilNextFramesUpdate <= 0)
            {
                mTimeUntilNextFramesUpdate = 1;
                CurrentFps = mDrawCalls;
                mDrawCalls = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                SaveGameState();
                mGame.GetStateManager().ChangeState(StateManager.StateType.PauseMenu);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                Win();
            }
            else
            {
                if (!(mGuiOpen is null))
                {
                    mGuiOpen.GuiHitboxCheck(this, managers.mInputHandler);
                }
                mGrid.Update(gameTime, this, managers.mSoundManager);
                mTiledMapRenderer.Update(gameTime);
                mCamera.Update_Cam(gameTime);
                managers.mSelectionManager.Update(gameTime, managers, this);
                if (mInputHandler.MouseClickRight)
                {
                    var gridDestination = mGrid.PositionToGridCoordinatesRelativeToCamera(mInputHandler.MousePosition);
                    var selectedObject = false;

                    if (!(mGuiOpen is null) && mGuiOpen.GuiHitboxCheck(this, managers.mInputHandler))
                    {
                        return;
                    }


                    for (var i = -1; i <= 1; i++)
                    {
                        if (selectedObject)
                        {
                            break;
                        }

                        for (var j = -1; j <= 1; j++)
                        {
                            var placePosition = new Point(gridDestination.X + i, gridDestination.Y + j);
                            if (mGrid.IsInGrid(placePosition))
                            {
                                var abstractGameObject = mGrid.GetAbstractGameObjectAt(placePosition);
                                if (managers.mInputHandler.Inputs.IsOpeningGuiInGrid(abstractGameObject.GetHitbox(),
                                        mGrid))
                                {
                                    if (!(mGuiOpen is null))
                                    {
                                        mGuiOpen.CloseGui();
                                        mButtonsAbstractGameObjectGui.Clear(); //delete buttons in neues schlittensegment, lager etc.
                                        mHasEnoughResourceForBuildingItems = ""; //for notification field
                                        mHasEnoughResourceForBuildingSledgeStation = ""; // for notificationfield
                                        mHasEnoughResourceForCookingFood = "";
                                    }

                                    abstractGameObject.OpenGui();
                                    mGuiOpen = abstractGameObject;
                                    selectedObject = true;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }

        internal override void PostUpdate(GameTime gameTime)
        {
        }

        public void Win()
        {
            mGame.StatisticsModel.mNumberOfGamesWon++;
            if (mGame.StatisticsModel.mFastestVictory < 0 || Timer < mGame.StatisticsModel.mFastestVictory)
            {
                mGame.StatisticsModel.mFastestVictory = Timer;
            }

            if (Timer <= 1200)
            {
                mGame.AchievementsModel.mRaceToTheNorthPole = true;
            }
            mGame.AchievementsModel.mVictory = true;
            mGame.GetSafeLoadManager().SaveStatistics(mGame.StatisticsModel);
            mGame.GetStateManager().ChangeState(StateManager.StateType.WinState);
        }

        //Creates a new, clear X x Y Grid
        private void InitGrid(int originX, int originY, int sizeX, int sizeY)
        {
            mGrid.InitGrid(originX, originY, sizeX, sizeY, mGrid);
        }

        //Adds any Object that inherits from AbstractGameObject to the Grid
        public void AddToGrid(int x, int y, AbstractGameObject gameObject)
        {
            mGrid.AddToGrid(x, y, gameObject);
        }

        public Point GetGridOffset()
        {
            return mGrid.GetGridOffset();
        }

        private void SaveGameState()
        {
            mGame.GetSafeLoadManager().SaveGameState(mGrid, this);
        }

        private bool LoadGameState()
        {
            return mGame.GetSafeLoadManager().LoadGameState(this, mGame.GetContentManager());
        }

        public GameDataModel GetGameDataModel()
        {
            var model = new GameDataModel
            {
                FleischCount = mResources.Get(ResourceType.Meat),
                MetallCount = mResources.Get(ResourceType.Metall),
                WoodCount = mResources.Get(ResourceType.Wood),
                KillCount = mKillCounter,
                KeyCount = mKeyCounter,
                Timer = Timer,
                EnemyFleischCount = mEnemyResources.Get(ResourceType.Meat),
                EnemyMetallCount = mEnemyResources.Get(ResourceType.Metall),
                EnemyWoodCount = mEnemyResources.Get(ResourceType.Wood),
                Tools = mEquipment,

            };
            return model;
        }

        public void ReadGameDataModel(GameDataModel model)
        {
            mResources.Increase(ResourceType.Meat, model.FleischCount);
            mResources.Increase(ResourceType.Metall, model.MetallCount);
            mResources.Increase(ResourceType.Wood, model.WoodCount);
            mEnemyResources.Increase(ResourceType.Meat, model.EnemyFleischCount);
            mEnemyResources.Increase(ResourceType.Metall, model.EnemyMetallCount);
            mEnemyResources.Increase(ResourceType.Wood, model.EnemyWoodCount);
            mKillCounter = model.KillCount;
            mKeyCounter = model.KeyCount;
            Timer = model.Timer;
            mEquipment = model.Tools;
            if (model.VictoryState == 1)
            {
                mGame.GetStateManager().ChangeState(StateManager.StateType.WinState);
            }
            if (model.VictoryState == 2)
            {
                mGame.GetStateManager().ChangeState(StateManager.StateType.GameOverState);
            }

        }

        public Grid GetCurrentGrid()
        {
            return mGrid;
        }

        public int GetGameStateId()
        {
            return mGameStateId;
        }

        public Camera GetCamera()
        {
            return mCamera;
        }

        public Generator GetGenerator()
        {
            return mGenerator;
        }

        public bool AbleToBuildHolzaxt(bool enemy)
        {
            var ressources = enemy ? mEnemyResources : mResources;
            return ressources.Get(ResourceType.Wood) >= QuantityOfWoodNeededForHolzaxt;
        }

        public bool AbleToBuildMetallaxt(bool enemy)
        {
            var ressources = enemy ? mEnemyResources : mResources;
            if (ressources.Get(ResourceType.Wood) >= QuantityOfMetallNeededForMetallaxt && mEquipment["Holzaxt"]>= QuantityHolzaxtForMetallaxt && mHasSchmiede)
            {
                return true;
            }
            return false;
        }
        public bool AbleToBuildHolzschwert(bool enemy)
        {
            var resources = enemy ? mEnemyResources : mResources;
            return resources.Get(ResourceType.Wood) >= QuantityOfWoodNeededForHolzschwert;
        }
        public bool AbleToBuildMetallschwert(bool enemy)
        {
            var resources = enemy ? mEnemyResources : mResources;
            if (resources.Get(ResourceType.Metall) >= QuantityOfMetallNeededForMetallschwert && mEquipment["Holzschwert"] >= QuantityHolzschwertForMetallschwert && mHasSchmiede)
            {
                return true;
            }
            return false;
        }
        public bool AbleToBuildHolzbogen(bool enemy)
        {
            var resources = enemy ? mEnemyResources : mResources;
            return resources.Get(ResourceType.Wood) >= QuantityOfWoodNeededForHolzbogen;
        }
        public bool AbleToBuildMetallbogen(bool enemy)
        {
            var resources = enemy ? mEnemyResources : mResources;
            if (resources.Get(ResourceType.Metall) >= QuantityOfMetallNeededForMetallbogen && mEquipment["Holzbogen"] >= QuantityHolzbogenForMetallbogen && mHasSchmiede)
            {
                return true;
            }
            return false;
        }
        public bool AbleToBuildMetallruestung(bool enemy)
        {
            var resources = enemy ? mEnemyResources : mResources;
            if (resources.Get(ResourceType.Metall) >= QuantityOfMetallNeededForMetallruestung && mHasSchmiede)
            {
                return true;
            }
            return false;
        }

        public void AddKill()
        {
            mKillCounter++;
            if (mKeyCounter == 3) { mKillsAfterKeys++; }
            if (!mGame.AchievementsModel.mFirstKill)
            {
                mGame.AchievementsModel.mFirstKill = true;
                mGame.GetSafeLoadManager().SaveAchievements(mGame.AchievementsModel);
            }
            else if (!mGame.AchievementsModel.mTenKills && mKillCounter >= 10)
            {
                mGame.AchievementsModel.mTenKills = true;
                mGame.GetSafeLoadManager().SaveAchievements(mGame.AchievementsModel);
            }
            else if (!mGame.AchievementsModel.mHardcore && mKillCounter >= 100)
            {
                mGame.AchievementsModel.mHardcore = true;
                mGame.GetSafeLoadManager().SaveAchievements(mGame.AchievementsModel);
            }
        }

        public void AddPlayerHuman(Human human)
        {
            mPlayerAliveHumans[human.Guid] = human;
            Debug.WriteLine(human.Guid);
            Debug.WriteLine(human.GetGridPos());
        }
        public void RemovePlayerHuman(Human human)
        {
            mPlayerAliveHumans.Remove(human.Guid);
            if (mPlayerAliveHumans.Count <= 0)
            {
                mGame.GetSafeLoadManager().SaveGameState(mGrid, this);
                mGame.GetStateManager().ChangeState(StateManager.StateType.GameOverState);
            }
        }
    }
}