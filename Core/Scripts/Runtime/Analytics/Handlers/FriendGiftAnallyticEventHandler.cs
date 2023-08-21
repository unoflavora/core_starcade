using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class FriendGiftAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string GIVE_COLLECTIBLE_PIN = "give_collectible_pin";
			public const string RESET_GIVE_COLLECTIBLE_LIMIT = "reset_give_collectible_limit";

		}

		public FriendGiftAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackGiveCollectiblePinEvent(GiveCollectiblePinParameterData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"total_pin", data.TotalPin },
				{"pins", data.Pins },
			};
			_analytic.LogEvent(ANALYTIC_KEY.GIVE_COLLECTIBLE_PIN);
		}

		public void TrackResetGiveCollectibleLimitEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.RESET_GIVE_COLLECTIBLE_LIMIT);
		}

		public class GiveCollectiblePinParameterData
		{
			public int TotalPin { get; set; }
			public List<PinParameterData> Pins { get; set; }
		}

		public class PinParameterData
		{
			public string Id { get; set; }
			public int Rarity { get; set; }
		}
	}
}
