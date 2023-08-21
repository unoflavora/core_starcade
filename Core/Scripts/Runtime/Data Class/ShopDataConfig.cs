using System;
using System.Collections.Generic;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Reward;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class ShopDataConfig
    {
        public string ItemId;
        public string ItemName;
        public long Cost;
        public CurrencyTypeEnum CostCurrency;
        public StoreItemTypeEnum Type;
        public float Interval;
        public StoreCategoryTypeEnum StoreCategory;
        public LootboxCategoryEnum LootboxCategory;
        public List<RewardBase> Items;
    }
}