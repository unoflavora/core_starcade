using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
    public enum StoreItemTypeEnum
    {
        [EnumMember(Value = "General")]
        General = 0,
        [EnumMember(Value = "Cooldown")]
        Cooldown = 1,
        [EnumMember(Value = "OneTimePurchase")]
        OneTimePurchase = 2,
    }
}