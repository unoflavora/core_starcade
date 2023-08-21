using System.Collections.Generic;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;

namespace Agate.Starcade.Runtime.Data
{
    public class ShopDataBuyResponse
    {
        public string ItemId;
        public RewardBase[] GrantedItems;
        public PayData Pay;
        public List<LootboxGachaResult> LootboxGachaResult;
    }

    public class PayData
    {
        public CurrencyTypeEnum Type;
        public double Amount;
    }
}