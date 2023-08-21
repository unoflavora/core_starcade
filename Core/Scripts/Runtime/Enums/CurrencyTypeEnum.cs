using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
	public enum CurrencyTypeEnum
	{
		[EnumMember(Value = "GoldCoin")]
		GoldCoin = 0,
		[EnumMember(Value = "StarCoin")]
		StarCoin = 1,
		[EnumMember(Value = "StarTicket")]
		StarTicket = 2,
	}
}
