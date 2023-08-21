using System;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class ShopData
    {
        public string itemId;
        public string itemName;
        public string lastPurchase;
        public float remainingSecond;
        public ShopDataConfig itemConfig;
    }
}