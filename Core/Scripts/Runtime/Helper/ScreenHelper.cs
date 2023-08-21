using UnityEngine;

namespace Agate.Starcade.Runtime.Helper
{
	public static class ScreenHelper
    {
		public static ScreenOrientation LastScreenOrientation { get; set; } = ScreenOrientation.LandscapeLeft;

        public static ScreenOrientation GetOrientation()
        {
			return LastScreenOrientation;
//#if UNITY_IOS
//			return LastScreenOrientation;
//			//if (Input.deviceOrientation == DeviceOrientation.Unknown || Screen.orientation != ScreenOrientation.Unknown)
//			//{
//			//	return Screen.orientation;
//			//}
			
//#endif

			switch (Input.deviceOrientation)
			{
				case DeviceOrientation.Portrait or DeviceOrientation.PortraitUpsideDown:
					return ScreenOrientation.Portrait;
				case DeviceOrientation.LandscapeLeft or DeviceOrientation.LandscapeRight:
				default:
					return ScreenOrientation.LandscapeLeft;
			}

		}
	}
}