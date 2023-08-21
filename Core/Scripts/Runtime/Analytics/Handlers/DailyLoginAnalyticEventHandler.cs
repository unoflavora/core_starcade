using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class DailyLoginAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string RECEIVE_DAILY_LOGIN_REWARD_EVENT = "receive_daily_login_reward";
		}

		public DailyLoginAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackReceiveDailyLoginReward(int day, string rewardType, decimal rewardAmount, string refId, object rewardObject = null)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"day", day },
				{"reward_type", rewardType },
				{"reward_amount", rewardAmount },
				{"ref", refId },
				{"reward_object", rewardObject },
			};

			_analytic.LogEvent(ANALYTIC_KEY.RECEIVE_DAILY_LOGIN_REWARD_EVENT, parameters);
		}


	}
}
