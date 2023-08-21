using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
	public enum ItemTypeEnum
	{
		[EnumMember(Value = "Avatar")]
		Avatar = 0,
		[EnumMember(Value = "Frame")]
		Frame = 1,
		[EnumMember(Value = "Background")]
		Background = 2,
		[EnumMember(Value = "Collectible")]
		Collectible = 3,
		[EnumMember(Value = "Currency")]
		Currency = 4,
		[EnumMember(Value = "Reward")]
		Reward = 5,
		[EnumMember(Value = "LootBoxChest")]
		LootBoxChest = 6,
		[EnumMember(Value = "Lootbox")]
		Lootbox = 7,
		[EnumMember(Value = "Social")]
		Social = 8,
		[EnumMember(Value = "Pets")] 
		Pets = 9,
        [EnumMember(Value = "Fragment")]
        Fragment = 10,
    }
}