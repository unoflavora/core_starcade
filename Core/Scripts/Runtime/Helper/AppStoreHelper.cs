using UnityEngine;

namespace Agate.Starcade.Runtime.Helper
{
	public static class AppStoreHelper
    { 
        public static void OpenGooglePlayStore()
        {
            Application.OpenURL($"market://details?id=com.extremejustice.starcadejackpotmania");
		}
		public static void OpenAppleAppStore()
		{
			//Application.OpenURL("market://details?id=com.extremejustice.starcadejackpotmania");
			Application.OpenURL($"macappstore://itunes.apple.com/us/developer/starcade-jackpot-mania/id6445887061");
		}
	}
}
