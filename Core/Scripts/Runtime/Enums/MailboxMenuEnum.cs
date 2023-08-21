using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Agate.Starcade.Runtime.Enums
{
	public enum MailboxMenuEnum
	{
		[EnumMember(Value = "Collect")]
		Collect = 0,
		[EnumMember(Value = "System")]
		System = 1,
		[EnumMember(Value = "Information")]
		Information = 2,
		[EnumMember(Value = "Community")]
		Community = 3,
	}
}
