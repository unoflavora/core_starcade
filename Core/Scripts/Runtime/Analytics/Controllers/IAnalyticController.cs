using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Analytics.Controllers
{
	public interface IAnalyticController
	{
		void Init();
		void SetUserId(string userId);
		void LogEvent(string eventName);
		void LogEvent<T>(string eventName, IDictionary<string, T> payloads = null);
		void LogEvent(string eventName, string parameterName, object parameterValue);
		void SetUserProperty(string key, string property);
		void SetUserProperty(IDictionary<string, string> properties = null);
		void Reset();
	}

}
