using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class AuthAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string CLICK_BIND_ACCOUNT = "click_bind_account";
			public const string BIND_ACCOUNT_SUCCESS = "bind_account_success";

			public const string ACCOUNT_TYPE_PARAM = "click_bind_account";

			public const string LOGOUT = "logout";

		}


		public AuthAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackClickBindAccountEvent(string accountType)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_BIND_ACCOUNT, ANALYTIC_KEY.ACCOUNT_TYPE_PARAM, accountType);
		}

		public void TrackBindAccountSuccessEvent(string accountType)
		{
			_analytic.LogEvent(ANALYTIC_KEY.BIND_ACCOUNT_SUCCESS, ANALYTIC_KEY.ACCOUNT_TYPE_PARAM, accountType);
		}

		public void TrackLogoutEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.LOGOUT);
		}
	}
}
