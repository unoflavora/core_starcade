using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using System.Collections.Generic;

namespace Agate.Starcade.Runtime.Data.DailyLogin
{
    [System.Serializable]
    public class DailyLoginRewardData
    {
        public string Caption;
        public int Day;
        public RewardEnum RewardType;
        public int Amount;
        public RewardStatusEnum IsClaim;
        public int Tier;
        public string Ref;
        public object RefObject;
    }

    public class PetRef
    {
        public string PetId { get; set; }
        public string PetName { get; set; }
    }

    public class LootboxRef
    {
        public string CollectibleSetId { get; set; }
        public string LootboxItemId { get; set; }
        public string LootboxItemName { get; set; }
        public int Index { get; set; }
        public string RarityType { get; set; }
        public LootboxRarityData RarityConfig { get; set; } = new LootboxRarityData();
    }

    public class LootboxRarityData
    {
        public string RarityId { get; set; }
        public List<ConfigModel> Configuration { get; set; }
        public int Amount { get; set; }
    }

    public class LootboxClaimRef
    {
        public List<LootboxGachaResult> LootboxGachaResults { get; set; }
        public LootboxRef LootboxData { get; set; }
    }

    public class ConfigModel
    {
        public int TargetRarity { get; set; }
        public float ChanceRate { get; set; }
    }

}
