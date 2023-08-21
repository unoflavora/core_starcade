using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
    public enum StoreCategoryTypeEnum
    {
        [EnumMember(Value = "General")]
        General = 0,
        [EnumMember(Value = "LimitedOffer")]
        LimitedOffer = 1,
        [EnumMember(Value = "LootBox")]
        LootBox = 2,
        [EnumMember(Value = "PetBox")]
        PetBox = 3,
    }
}