using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class MailboxAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string CLICK_MAILBOX_COLLECT_TAB = "click_mailbox_collect_tab";
			public const string CLICK_MAILBOX_SYSTEM_TAB = "click_mailbox_system_tab";
			public const string CLICK_MAILBOX_INFORMATION_TAB = "click_mailbox_information_tab";
			public const string CLICK_MAILBOX_COMMUNITY_TAB = "click_mailbox_community_tab";
			public const string COLLECT_ALL_MAILBOX_ITEMS = "collect_all_mailbox_items";
			public const string OPEN_MAILBOX_ITEM = "open_mailbox_item";
			public const string COLLECT_MAILBOX_ITEMS = "collect_mailbox_items";
		}

		public MailboxAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickMailboxCollectTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MAILBOX_COLLECT_TAB);
		}

		public void TrackClickMailboxSystemTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MAILBOX_SYSTEM_TAB);
		}

		public void TrackClickMailboxInformationTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MAILBOX_INFORMATION_TAB);
		}

		public void TrackClickMailboxCommunityTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MAILBOX_COMMUNITY_TAB);
		}

		public void TrackCollectAllMailboxItemsEvent(List<string> ids)
		{
			_analytic.LogEvent(ANALYTIC_KEY.COLLECT_ALL_MAILBOX_ITEMS, "ids", ids);
		}

		public void TrackOpenMailboxItem(string tab, string id)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"tab", tab },
				{"id", id },
			};

			_analytic.LogEvent(ANALYTIC_KEY.OPEN_MAILBOX_ITEM, parameters);
		}

		public void TrackCollectMailboxItemsEvent(string id, List<RewardParameters> rewardData)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"item_id", id },
				{"reward_object", rewardData },
			};
			for (int i = 0; i < rewardData.Count; i++)
			{
				var reward = rewardData[i];

				parameters.Add($"reward_{i + 1}_type", reward.Type);
				parameters.Add($"reward_{i + 1}_amount", reward.Amount);
				parameters.Add($"reward_{i + 1}_ref", reward.Ref);
			}

			_analytic.LogEvent(ANALYTIC_KEY.COLLECT_MAILBOX_ITEMS, parameters);
		}
		public class RewardParameters
		{
			public string Ref { get; set; }
			public string Type { get; set; }
			public float Amount { get; set; }
		}
	}
}
