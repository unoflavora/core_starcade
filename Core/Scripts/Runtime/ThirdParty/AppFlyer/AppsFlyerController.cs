using UnityEngine;
using AppsFlyerSDK;

// This class is intended to be used the the AppsFlyerObject.prefab
namespace Agate.Starcade.Core.Runtime.ThirdParty.AppFlyer
{ 
    public class AppsFlyerController 
    {
        private AppFlyerConfig _config { get; set; }
        private AppFlyerConversionDataHandler _conversionDataHandler { get; set; }

        public AppsFlyerController(AppFlyerConfig config, AppFlyerConversionDataHandler conversionDataHandler = null)
        {
			_config = config;

            if(_config.GetConversionData)
            {
                _conversionDataHandler = conversionDataHandler;
            }
		}

        public void Init()
        {
			if (!_config.IsEnabled) return;

			// These fields are set from the editor so do not modify!
			//******************************//
			AppsFlyer.setIsDebug(_config.IsDebug);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
            AppsFlyer.initSDK(_config.DevKey, _config.UWPAppID, _conversionDataHandler);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
        AppsFlyer.initSDK(_config.DevKey, _config.AppID, _conversionDataHandler);
#else
			AppsFlyer.initSDK(_config.DevKey, _config.AppID, _conversionDataHandler);
#endif
			//******************************/

			AppsFlyer.startSDK();

            Debug.Log("AppFlyer is Initialized!");
		}
    }
}