using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Data.DailyLogin
{
    public enum DailyLoginRewardEnum
    {
        [EnumMember(Value = "GoldCoin")]
        GoldCoin = 0,
        [EnumMember(Value = "StarCoin")]
        StarCoin = 1,
        [EnumMember(Value = "StarTicket")]
        StarTicket = 2,
        [EnumMember(Value = "Lootbox")]
        LootBox = 3,
        [EnumMember(Value = "Pet")]
        Pet = 4,
      
    }
}
