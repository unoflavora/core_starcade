using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Firebase.Analytics;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.ThirdParty.Firebase
{
    public class FirebaseAnalyticController : IAnalyticController
	{ 
		public void Init()
        {
			FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
		}

		public void LogEvent<T>(string eventName, IDictionary<string, T> payloads = null)
		{
			if (payloads == null) return;

			List<Parameter> param = new List<Parameter>();
			T value;
			foreach (var v in payloads)
			{
				value = v.Value;
				if (value is string)
				{
					param.Add(new Parameter(v.Key, value.ToString()));
				}
				else if (value is double || value is float)
				{
					param.Add(new Parameter(v.Key, Convert.ToDouble(value)));
				}
				else if (value is int || value is long)
				{
					param.Add(new Parameter(v.Key, Convert.ToInt64(value)));
				}
				else
				{
					param.Add(new Parameter(v.Key, JsonConvert.SerializeObject(value)));
				}
			}

			UnityEngine.Debug.Log($"ANALYTIC LOG EVENT [{eventName}]: {JsonConvert.SerializeObject(payloads)}");
			FirebaseAnalytics.LogEvent(eventName, param.ToArray());
		}

		public void SetUserId(string userId)
		{
			FirebaseAnalytics.SetUserId(userId);
			//Crashlytics.SetUserId(userId);
		}

		public void Reset()
		{
			FirebaseAnalytics.ResetAnalyticsData();
		}

		public void SetUserProperty(string key, string property)
		{
			FirebaseAnalytics.SetUserProperty(key, property);
			UnityEngine.Debug.Log($"ANALYTIC USER PROPERTY: {key}|{property}");
		}

		public void SetUserProperty(IDictionary<string, string> properties = null)
		{
			if (properties == null) return;

			foreach (var p in properties)
			{
				FirebaseAnalytics.SetUserProperty(p.Key, p.Value);
				UnityEngine.Debug.Log($"ANALYTIC USER PROPERTY: {p.Key}|{p.Value}");
			}
		}

		public void LogEvent(string eventName)
		{
			FirebaseAnalytics.LogEvent(eventName);
			UnityEngine.Debug.Log($"ANALYTIC LOG EVENT [{eventName}]");
		}

		public void LogEvent(string eventName, string parameterName, object parameterValue)
		{
			if(parameterValue is string)
			{
				FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue.ToString());
			}
			else if (parameterValue is double || parameterValue is float || parameterValue is decimal)
			{
				FirebaseAnalytics.LogEvent(eventName, parameterName, Convert.ToDouble(parameterValue));
			}
			else if (parameterValue is int || parameterValue is long)
			{
				FirebaseAnalytics.LogEvent(eventName, parameterName, Convert.ToInt64(parameterValue));
			}
			else
			{
				FirebaseAnalytics.LogEvent(eventName, parameterName, JsonConvert.SerializeObject(parameterValue));
			}

			UnityEngine.Debug.Log($"ANALYTIC LOG EVENT [{eventName}]:  {parameterName}|{JsonConvert.SerializeObject(parameterValue)}");
		}

	}
}
