using Agate.Starcade.Core.Runtime.Analytics.Controllers;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class PermissionAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string OPEN_PERMISSION_EVENT = "open_permission";
			public const string ACCEPT_PERMISSION_EVENT = "fftue_permission";

			public const string CLICK_TERMS_OF_SERVICE_EVENT = "click_terms_of_service";
			public const string CLICK_PRIVACY_POLICY_EVENT = "click_privacy_policy";
		}


		public PermissionAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackOpenPermissionEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.OPEN_PERMISSION_EVENT);
		}

		public void TrackAcceptPermissionEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.ACCEPT_PERMISSION_EVENT);
		}

		public void TrackClickTermsOfServiceEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_TERMS_OF_SERVICE_EVENT);
		}

		public void TrackClickPrivacyPolicyEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PRIVACY_POLICY_EVENT);
		}
	}
}
