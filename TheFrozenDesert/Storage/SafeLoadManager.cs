using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TheFrozenDesert.Content;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.Storage
{
    public sealed class SafeLoadManager
    {
        public void SaveGameState(Grid grid, GameState gameState)
        {
            DirectoryManager.CreateGamestateDirectoryIfEmpty(gameState.GetGameStateId().ToString());
            var toSave = grid.ReturnAllSaveableObjects();
            using var gridWriter = new StreamWriter(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("GameStates/" + gameState.GetGameStateId() + "/ResourceObjects.json"),
                FileMode.Create));
            gridWriter.WriteLine("[");
            string serializedText;
            foreach (var abstractGameObject in toSave)
            {
                serializedText = JsonConvert.SerializeObject(abstractGameObject.Serialize(gameState),
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                gridWriter.WriteLine(serializedText);
                gridWriter.WriteLine(",");
            }
            foreach (var wolfPack in gameState.mWolfPacks)
            {
                serializedText = JsonConvert.SerializeObject(wolfPack.Serialize(),
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                gridWriter.WriteLine(serializedText);
                gridWriter.WriteLine(",");
            }
            gridWriter.WriteLine("]");
            gridWriter.Flush();
            gridWriter.Close();
            using var gameDataWriter = new StreamWriter(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("GameStates/" + gameState.GetGameStateId() + "/GameData.json"),
                FileMode.Create));
            serializedText = JsonConvert.SerializeObject(gameState.GetGameDataModel(),
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            gameDataWriter.WriteLine(serializedText);
        }

        public bool LoadGameState(GameState gameState, FrozenDesertContentManager manager)
        {
            if (!Directory.Exists(DirectoryManager.CombineBaseDirectoryWithFile("GameStates/" + gameState.GetGameStateId() + "/")))
            {
                return false;
            }

            var postLoadManager = new PostLoadManager();
            using var gridReader = new StreamReader(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("GameStates/" + gameState.GetGameStateId() + "/ResourceObjects.json"),
                FileMode.Open));
            {
                var abstractGameObjectModels = JsonConvert.DeserializeObject<List<AbstractGameObjectModel>>(
                    gridReader.ReadToEnd(),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                if (abstractGameObjectModels != null)
                {
                    foreach (var abstractGameObjectModel in abstractGameObjectModels)
                    {
                        switch (abstractGameObjectModel)
                        {
                            case ResourceObjectModel model:
                            {
                                var resourceObject = new ResourceObject(gameState, model, gameState.GetCurrentGrid());
                                gameState.AddToGrid(resourceObject.GetGridPos().X,
                                    resourceObject.GetGridPos().Y,
                                    resourceObject);
                                break;
                            }
                            case ArcherModel archerModel:
                                gameState.GamePlayObjectFactory.CreateArcher(archerModel);
                                break;
                            case FighterModel fighterModel:
                                gameState.GamePlayObjectFactory.CreateFighter(fighterModel);
                                break;
                            case GathererModel gathererModel:
                                gameState.GamePlayObjectFactory.CreateGatherer(gathererModel);
                                break;
                            case CampfireModel campfireModel:
                                gameState.GamePlayObjectFactory.CreateCampfire(campfireModel);
                                break;
                            case WolfModel wolfModel:
                            {
                                Wolf wolf = gameState.GamePlayObjectFactory.CreateWolf(wolfModel);
                                postLoadManager.mUuidToWolves[wolfModel.Uuid] = wolf;
                                break;
                            }
                            case WolfPackModel wolfPackModel:
                                postLoadManager.mWolfPackModels.Add(wolfPackModel);
                                break;
                            case SledgeModel sledgeModel:
                            {
                                var sledge = sledgeModel.CreateSledge(gameState, manager);
                                gameState.GetCamera().ActivateCenterObjekt(sledge);
                                postLoadManager.mUuidToSledges.Add(sledgeModel.Uuid, sledge);
                                gameState.Sledge = sledge;
                                if (!sledgeModel.PreviousSledgeUuid.Equals("none"))
                                {
                                    postLoadManager.mSledgeToPreviousSledgeUuid.Add(sledge, sledgeModel.PreviousSledgeUuid);
                                }

                                gameState.AddToGrid(sledgeModel.X, sledgeModel.Y, sledge);
                                break;
                            }
                            case BridgeModel bridgeModel:
                                gameState.GamePlayObjectFactory.CreateBridge(bridgeModel);
                                break;
                            case KeyModel keyModel:
                                gameState.GamePlayObjectFactory.CreateKey(keyModel);
                                break;
                            case SnowTileModel snowTileModel:
                                gameState.GamePlayObjectFactory.CreateSnowTile(snowTileModel);
                                break;
                            case BigTreeModel bigTreeModel:
                                gameState.GamePlayObjectFactory.CreateBigTree(bigTreeModel.GridCurrentPositionX, bigTreeModel.GridCurrentPositionY, "Um Die Welt aufzutauen, musst du einen Reaktor im Westen aktivieren (Sammle alle Schluessel)");
                                break;
                        }
                    }
                }
            }
            gridReader.Close();
            using var gameDataReader = new StreamReader(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("GameStates/" + gameState.GetGameStateId() + "/GameData.json"),
                FileMode.Open));
            var gameDataModel = JsonConvert.DeserializeObject<GameDataModel>(gameDataReader.ReadToEnd(),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            gameState.ReadGameDataModel(gameDataModel);
            postLoadManager.Run(gameState);
            return true;
        }

        public void SaveAchievements(AchievementsModel model)
        {
            DirectoryManager.CreateAchievementDirectoryIfEmpty();
            using var achievementsWriter = new StreamWriter(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("Achievements" + "/Achievements.json"),
                FileMode.Create));
            string serializedText = JsonConvert.SerializeObject(model,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            achievementsWriter.WriteLine(serializedText);
        }

        public void SaveStatistics(StatisticsModel model)
        {
            DirectoryManager.CreateStatisticsDirectoryIfEmpty();
            using var statisticsWriter = new StreamWriter(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("Statistics" + "/Statistics.json"),
                FileMode.Create));
            string serializedText = JsonConvert.SerializeObject(model,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            statisticsWriter.WriteLine(serializedText);
        }

        public StatisticsModel LoadStatisticsModel()
        {
            if (!Directory.Exists(DirectoryManager.CombineBaseDirectoryWithFile("Statistics" + "/")))
            {
                return new StatisticsModel();
            }

            using var statisticsReader = new StreamReader(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("Statistics" + "/Statistics.json"),
                FileMode.Open));
            StatisticsModel statisticsModel = JsonConvert.DeserializeObject<StatisticsModel>(
                statisticsReader.ReadToEnd(),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            return statisticsModel;
        }


        public AchievementsModel LoadAchievements()
        {
            if (!Directory.Exists(DirectoryManager.CombineBaseDirectoryWithFile("Achievements" + "/")))
            {
                return new AchievementsModel();
            }

            using var achievementsReader = new StreamReader(new FileStream(
                DirectoryManager.CombineBaseDirectoryWithFile("Achievements" + "/Achievements.json"),
                FileMode.Open));
            AchievementsModel achievementsModel = JsonConvert.DeserializeObject<AchievementsModel>(
                achievementsReader.ReadToEnd(),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            return achievementsModel;
        }

        public List<GameSaveModel> LoadGameSaves()
        {
            List<GameSaveModel> modelList = new List<GameSaveModel>();
            string path = DirectoryManager.CombineBaseDirectoryWithFile("GameStates/");
            foreach (string directory in Directory.GetDirectories(path))
            {
                GameSaveModel model = new GameSaveModel()
                {
                    mDateTime = Directory.GetCreationTime(directory),
                    mSeed = Int32.Parse(directory.Remove(0, path.Length))
                };
                modelList.Add(model);
            }
            List<GameSaveModel> returnList = modelList.OrderByDescending(m => m.mDateTime).ToList();
            return returnList;
        }
        public List<GameDataModel> LoadGameDataModelsModelsForStatistics()
        {
            List<GameDataModel> modelList = new List<GameDataModel>();
            string path = DirectoryManager.CombineBaseDirectoryWithFile("GameStates/");
            if (Directory.Exists(path))
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    using var gameDataReader = new StreamReader(new FileStream(
                        DirectoryManager.CombineBaseDirectoryWithFile(directory + "/GameData.json"),
                        FileMode.Open));
                    var gameDataModel = JsonConvert.DeserializeObject<GameDataModel>(gameDataReader.ReadToEnd(),
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });
                    modelList.Add(gameDataModel);
                }
            }
            return modelList;
        }
    }
}