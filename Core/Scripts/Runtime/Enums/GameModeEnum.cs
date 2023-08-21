using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Agate.Starcade.Runtime.Enums
{
	public enum GameModeEnum
	{
		[EnumMember(Value = "Gold")]
		Gold = 0,
		[EnumMember(Value = "Star")]
		Star = 1,
	}
}