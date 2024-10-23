using System.Collections.Generic;
using System.Drawing;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public sealed class WolfPack
    {
        private readonly List<Wolf> mWolves;
        private Rectangle mWolfArea;

        internal Rectangle WolfArea => mWolfArea;

        internal WolfPack(Rectangle wolfArea)
        {
            mWolves = new List<Wolf>();
            mWolfArea = wolfArea;
        }

        internal WolfPack(WolfPackModel wolfPackModel)
        {
            mWolves = new List<Wolf>();
            mWolfArea = new Rectangle(wolfPackModel.AreaX, wolfPackModel.AreaY, wolfPackModel.SizeX, wolfPackModel.SizeY);
        }

        internal void Add(Wolf wolf)
        {
            mWolves.Add(wolf);
        }

        internal WolfPackModel Serialize()
        {
            string[] buffer = new string[mWolves.Count];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = mWolves[i].Uuid.ToString();
            }
            return new WolfPackModel()
            {
                Uuids = buffer,
                AreaX = mWolfArea.X,
                AreaY = mWolfArea.Y,
                SizeX = mWolfArea.Size.Width,
                SizeY = mWolfArea.Size.Height
            };
        }

    }
}