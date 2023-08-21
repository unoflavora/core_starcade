using Agate.Starcade.Runtime.Enums;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade
{
    public class ItemAccessoryData
    {
        public string Id;
        public ItemTypeEnum Type;
        public bool IsLocked;
        public bool IsLoaded;
        public bool IsHighlighted;
        public bool UseNullData;
        public AssetReference Reference;
        public object Data;
    }
}
