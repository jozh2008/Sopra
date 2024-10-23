using System;
using System.Collections.Generic;
using TheFrozenDesert.GamePlayObjects;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.Storage
{
    internal sealed class PostLoadManager
    {
        public readonly Dictionary<Sledge, string> mSledgeToPreviousSledgeUuid;
        public readonly Dictionary<string, Sledge> mUuidToSledges;
        public readonly List<WolfPackModel> mWolfPackModels;
        public readonly Dictionary<string, Wolf> mUuidToWolves;

        public PostLoadManager()
        {
            mUuidToSledges = new Dictionary<string, Sledge>();
            mSledgeToPreviousSledgeUuid = new Dictionary<Sledge, string>();
            mWolfPackModels = new List<WolfPackModel>();
            mUuidToWolves = new Dictionary<string, Wolf>();
        }

        public void Run(GameState gameState)
        {

            try
            {
                foreach (var model in mWolfPackModels)
                {
                    WolfPack pack = new WolfPack(model);
                    foreach (var uuid in model.Uuids)
                    {
                        pack.Add(mUuidToWolves[uuid]);
                        mUuidToWolves[uuid].AddToPack(pack);
                    }
                    gameState.mWolfPacks.Add(pack);
                }
                foreach (var pair in mSledgeToPreviousSledgeUuid)
                {
                    pair.Key.SetPreviousSledge(mUuidToSledges[pair.Value]);
                }
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}