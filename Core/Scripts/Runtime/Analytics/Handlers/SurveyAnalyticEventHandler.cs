using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class SurveyAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_SURVEY = "click_survey";

			public const string CLICK_LOCATION_PARAM = "click_location";

		}

		public SurveyAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickSurvey(string click_location)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_SURVEY, ANALYTIC_KEY.CLICK_LOCATION_PARAM, click_location);
		}

	}
}
