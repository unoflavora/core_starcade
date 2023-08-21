using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class OnboardingAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string BASE_ONBOARDING_KEY = "fftue";
		}

		public OnboardingAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackOnboardingEvent(string field, int index)
		{
			_analytic.LogEvent($"{ANALYTIC_KEY.BASE_ONBOARDING_KEY}_{field}_{index}");
		}

		public void TrackStartOnboardingEvent(string field)
		{
			_analytic.LogEvent($"{ANALYTIC_KEY.BASE_ONBOARDING_KEY}_{field}_start");
		}

		public void TrackEndOnboardingEvent(string field)
		{
			_analytic.LogEvent($"{ANALYTIC_KEY.BASE_ONBOARDING_KEY}_{field}_complete");
		}

		public void TrackSkipOnboardingEvent(string field, int index)
		{
			_analytic.LogEvent($"{ANALYTIC_KEY.BASE_ONBOARDING_KEY}_{field}_skip", "index", index);
		}
	}
}
