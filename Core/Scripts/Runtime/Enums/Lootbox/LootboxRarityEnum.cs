using System.Runtime.Serialization;

namespace Agate.Starcade.Scripts.Runtime.Enums.Lootbox
{
    public enum LootboxRarityEnum
    {
        [EnumMember(Value = "bronze")]
        Bronze,
        [EnumMember(Value = "silver")]
        Silver,
        [EnumMember(Value = "gold")]
        Gold
    }
}