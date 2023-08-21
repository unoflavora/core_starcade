using Agate.Starcade.Core.Runtime.ThirdParty.AppFlyer;
using System;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Config
{
    [CreateAssetMenu(fileName = "ThirdPartyConfig", menuName = "config/add new third party config", order = 0)]
    public class ThirdPartyConfig : ScriptableObject
	{
		public GoogleConfig Google;
		public AppFlyerConfig AppFlyer;
    }

	[Serializable]
	public class GoogleConfig
	{
		public string GoogleWebClientId;
		public string DummyGoogleAuthToken;
	}
}
