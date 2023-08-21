using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class SplashScreenAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string SPLASH_SCREEN_EVENT = "fftue_splashScreen";
		}


		public SplashScreenAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		
		public void TrackSplashScreenEvent()
		{
			_analytic.LogEvent(FirebaseAnalytics.EventLogin);
		}

	}
}
