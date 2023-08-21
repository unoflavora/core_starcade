using System;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Lobby.Store
{
    [Serializable]
    public class GeneralStoreModel
    {
        public bool IsLocked;
        public string ItemId;

        public double Gold;
        public double Star;

        public double Cost;
        public string CostAsset;
        [HideInInspector] public Sprite CostSprite;

        public string BackgroundAssetId;
        [HideInInspector] public Sprite Background;
        public string ItemAssetId;
        [HideInInspector] public Sprite Item;
    }
}
