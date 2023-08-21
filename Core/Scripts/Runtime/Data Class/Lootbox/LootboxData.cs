using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;

namespace Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox
{
    public class LootboxData
    {
        public string CollectibleSetId { get; set; }
        public string LootboxItemId { get; set; }
        
        public int Index { get; set; }
        public LootboxRarityEnum RarityType { get; set; }
        public RarityConfig RarityConfig { get; set; }

        public bool IsPremium { get; set; } = false;
    }
}