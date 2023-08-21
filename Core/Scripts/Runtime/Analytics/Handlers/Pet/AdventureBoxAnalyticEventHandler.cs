using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class AdventureBoxAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string CLICK_OPEN_ADVENTURE_BOX_BUTTON_EVENT = "click_open_adventure_box_button";
			public const string CLICK_GIVE_ADVENTURE_BOX_BUTTON_EVENT = "click_give_adventure_box_button";
			public const string OPEN_ADVENTURE_BOX_EVENT = "open_adventure_box";
		}

		public AdventureBoxAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void TrackClickOpenAdventureBoxButtonEvent(string id)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_OPEN_ADVENTURE_BOX_BUTTON_EVENT, "adventure_box_id", id);
		}

		public void TrackClickGiveAdventureBoxButtonEvent(string id)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_GIVE_ADVENTURE_BOX_BUTTON_EVENT, "adventure_box_id", id);
		}

		public void TrackOpenAdventureBoxEvent(string id, int amount, List<RewardParameterData> rewards)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"adventure_box_id", id },
				{"amount", amount },
				{"rewards", rewards },
			};
			_analytic.LogEvent(ANALYTIC_KEY.OPEN_ADVENTURE_BOX_EVENT, parameters);
		}

		public class RewardParameterData
		{
			public string Type { get; set; }
			public float Amount { get; set; }
			public RewardParameterData(string type, float amount) { Type = type; Amount = amount; }
		}
	}
}