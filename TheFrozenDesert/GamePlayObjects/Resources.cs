using System;
using System.Collections.Generic;

namespace TheFrozenDesert.GamePlayObjects
{
    public enum ResourceType
    {
        Wood,
        Metall,
        Meat,
        Null
    }

    public sealed class Resources
    {
        private readonly Dictionary<ResourceType, int> mResources;

        public Dictionary<ResourceType, int> ResourceDictionary => mResources;


        public Resources()
        {
            mResources = new Dictionary<ResourceType, int>
            {
                {
                    ResourceType.Wood, 60
                },
                {
                    ResourceType.Metall, 40
                },
                {
                    ResourceType.Meat, 0
                }
            };
        }

        public int Get(ResourceType resourceType)
        {
            return mResources[resourceType];
        }

        public void Increase(ResourceType resourceType, int value)
        {
            mResources[resourceType] += value;
        }

        public void Decrease(ResourceType resourceType, int value)
        {
            var resources = mResources[resourceType] - value;
            if (resources >= 0)
            {
                mResources[resourceType] = resources;
            }
        }

        public String GetNameAsString(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    return "Holz";
                case ResourceType.Metall:
                    return "Metall";
                case ResourceType.Meat:
                    return "Fleisch";
            }

            return "";
        }

    }
}