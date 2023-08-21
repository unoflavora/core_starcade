using Agate.Starcade.Core.Runtime.ThirdParty.Firebase.Data;
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.ThirdParty.Firebase
{
    public class FirebaseController 
    {
		private DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
		protected bool _isInitialized { get; set; } = false;
		protected FirebaseAnalyticController _analytic { get; set; } = new FirebaseAnalyticController();
		protected FirebaseRemoteConfigController _remoteConfig { get; set; } = new FirebaseRemoteConfigController();

		#region Properties
		public bool IsInitialized => _isInitialized;
		public FirebaseAnalyticController Analytic => _analytic;
		public FirebaseRemoteConfigController RemoteConfig => _remoteConfig;
		public RemoteConfigData RemoteConfigData => _remoteConfig.Data;
		#endregion

		public void Init()
        {
			FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
				_dependencyStatus = task.Result;
				if (_dependencyStatus == DependencyStatus.Available)
				{
					InitializeFirebase();
				}
				else
				{
					Debug.LogError(
					  "Could not resolve all Firebase dependencies: " + _dependencyStatus);
				}
			});
		}

        private void InitializeFirebase()
        {
			Debug.Log("Enabling data collection.");
			_analytic.Init();
			_remoteConfig.Init();
			Crashlytics.IsCrashlyticsCollectionEnabled = true;

			_isInitialized = true;
		}


	}
}
