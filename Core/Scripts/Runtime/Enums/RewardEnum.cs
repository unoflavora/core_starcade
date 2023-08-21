using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
	public enum RewardEnum
	{
        [EnumMember(Value = "GoldCoin")]
        GoldCoin = 0,
        [EnumMember(Value = "StarCoin")]
        StarCoin = 1,
        [EnumMember(Value = "StarTicket")]
        StarTicket = 2,
        [EnumMember(Value = "Avatar")]
        Avatar = 3,
        [EnumMember(Value = "Frame")]
        Frame = 4,
        [EnumMember(Value = "Lootbox")]
        Lootbox = 5,
        [EnumMember(Value = "Collectible")]
        Collectible = 6,
        [EnumMember(Value = "Pet")]
        Pet = 7,
        [EnumMember(Value = "PetFragment")]
        PetFragment = 8,
        [EnumMember(Value = "SpecialBox")]
        SpecialBox = 9,
        [EnumMember(Value = "PetBox")]
        PetBox = 10
    }
}