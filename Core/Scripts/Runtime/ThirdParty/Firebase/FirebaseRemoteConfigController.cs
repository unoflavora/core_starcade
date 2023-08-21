using Firebase.RemoteConfig;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Agate.Starcade.Core.Runtime.ThirdParty.Firebase.Data;
using Newtonsoft.Json;

namespace Agate.Starcade.Core.Runtime.ThirdParty.Firebase
{
    public class FirebaseRemoteConfigController 
	{
		private RemoteConfigData _data { get; set; }

		#region Properties
		public RemoteConfigData Data => _data;
		#endregion

		// Initialize remote config, and set the default values.
		public async void Init()
        {
			_data = new RemoteConfigData();

			// [START set_defaults]
			System.Collections.Generic.Dictionary<string, object> defaults =
			  new System.Collections.Generic.Dictionary<string, object>();

			// These are the values that are used if we haven't fetched data from the
			// server
			// yet, or if we ask for values that the server doesn't have:
			defaults.Add(RemoteConfigKeys.PET_3D_ENABLED, _data.IsPet3DEnabled);
			defaults.Add(RemoteConfigKeys.CP01_REALTIME_ENABLED, _data.IsCP01RealtimeEnabled);

			await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
			  .ContinueWithOnMainThread(task =>
			  {
				  // [END set_defaults]
				  Debug.Log("RemoteConfig configured and ready!");
			  });

			await FetchDataAsync();
		}

		// [START fetch_async]
		// Start a fetch request.
		// FetchAsync only fetches new data if the current data is older than the provided
		// timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
		// By default the timespan is 12 hours, and for production apps, this is a good
		// number. For this example though, it's set to a timespan of zero, so that
		// changes in the console will always show up immediately.
		public Task FetchDataAsync()
		{
			Debug.Log("Fetching remote config data...");
			System.Threading.Tasks.Task fetchTask =
			FirebaseRemoteConfig.DefaultInstance.FetchAsync(
				TimeSpan.Zero);
			return fetchTask.ContinueWithOnMainThread(FetchComplete);
		}

		void FetchComplete(Task fetchTask)
		{
			if (fetchTask.IsCanceled)
			{
				Debug.Log("Fetch remote config canceled.");
			}
			else if (fetchTask.IsFaulted)
			{
				Debug.Log("Fetch remote config encountered an error.");
			}
			else if (fetchTask.IsCompleted)
			{
				Debug.Log("Fetch remote config completed successfully!");
			}

			var info = FirebaseRemoteConfig.DefaultInstance.Info;
			switch (info.LastFetchStatus)
			{
				case LastFetchStatus.Success:
					FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
					.ContinueWithOnMainThread(task => {
						Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
								   info.FetchTime));
						Debug.Log(JsonConvert.SerializeObject(FirebaseRemoteConfig.DefaultInstance.AllValues.Keys));

						_data.IsPet3DEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKeys.PET_3D_ENABLED).BooleanValue;
						_data.IsCP01RealtimeEnabled = FirebaseRemoteConfig.DefaultInstance.GetValue(RemoteConfigKeys.CP01_REALTIME_ENABLED).BooleanValue;

						Debug.Log(JsonConvert.SerializeObject(_data));
					});

					break;
				case LastFetchStatus.Failure:
					switch (info.LastFetchFailureReason)
					{
						case FetchFailureReason.Error:
							Debug.Log("Fetch failed for unknown reason");
							break;
						case FetchFailureReason.Throttled:
							Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
							break;
					}
					break;
				case LastFetchStatus.Pending:
					Debug.Log("Latest Fetch call still pending.");
					break;
			}
		}

		public object GetValue(string remoteConfigName)
		{
			return FirebaseRemoteConfig.DefaultInstance
			   .GetValue(remoteConfigName);
		}
	}
}
