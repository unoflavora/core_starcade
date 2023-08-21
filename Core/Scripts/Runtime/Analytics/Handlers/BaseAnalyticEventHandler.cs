using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class BaseAnalyticEventHandler 
    {
        protected IAnalyticController _analytic { get; set;}
		protected bool _isEnabled { get; set; } = true;
		public BaseAnalyticEventHandler(IAnalyticController analytic, bool isEnabled = true)
        {
            _analytic = analytic;
			_isEnabled = isEnabled;
		}

		protected void LogEvent(string eventName, IDictionary<string, object> payloads = null)
		{
			if (!_isEnabled) return;
			_analytic.LogEvent(eventName, payloads);
		}

		protected void LogEvent(string eventName, string parameterName, object parameterValue)
		{
			if (!_isEnabled) return;
			_analytic.LogEvent(eventName, parameterName, parameterValue);
		}
	}
}	
