using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class PosterAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string CLICK_POSTER_ITEM_EVENT = "click_poster_item";
			public const string CLAIM_POSTER_ITEM_REWARD = "claim_poster_item_reward";

			public const string ITEM_ID_PARAM = "item_id";

		}

		public PosterAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickPosterItem(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_POSTER_ITEM_EVENT, ANALYTIC_KEY.ITEM_ID_PARAM, itemId); ;
		}

		public void TrackClaimRewardEvent(string itemId, List<RewardParameters> rewardData)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"item_id", itemId },
				{"reward_object", rewardData },
			};
			for (int i = 0; i < rewardData.Count; i++)
			{
				var reward = rewardData[i];
				
				parameters.Add($"reward_{i+1}_type", reward.Type);
				parameters.Add($"reward_{i+1}_amount", reward.Amount);
			}

			_analytic.LogEvent(ANALYTIC_KEY.CLAIM_POSTER_ITEM_REWARD, parameters);
		}

		public class RewardParameters
		{
			public string Type { get; set; }
			public float Amount { get; set; }
		}
	}
}
