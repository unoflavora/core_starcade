using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Agate.Starcade.Runtime.Enums
{
	public enum TutorialModeEnum
	{
		[EnumMember(Value = "None")]
		None = 0,
		[EnumMember(Value = "Onboarding")]
		Onboarding = 1,
		[EnumMember(Value = "Tutorial")]
		Tutorial = 2,
	}
}