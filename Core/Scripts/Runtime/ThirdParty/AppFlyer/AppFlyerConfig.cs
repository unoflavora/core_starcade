using System;

namespace Agate.Starcade.Core.Runtime.ThirdParty.AppFlyer
{
	[Serializable]
	public class AppFlyerConfig
	{
		public bool IsEnabled = true;
		public string DevKey;
		public string AppID;
		public string UWPAppID;
		public string MacOSAppID;
		public bool IsDebug;
		public bool GetConversionData;
	}
}