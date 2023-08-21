using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class MyPetAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string CLICK_RECALL_BUTTON_EVENT = "click_recall_button";
			public const string CLICK_ADVENTURE_BUTTON_EVENT = "click_adventure_button";
			public const string CLICK_SWITCH_PET_BUTTON = "click_switch_pet_button";
			public const string SWITCH_PET_EVENT = "switch_pet";
			public const string CLICK_PET_STATUS_BUTTON = "click_pet_status_button";

			public const string TOUCH_PET_EVENT = "touch_pet";

		}

		public MyPetAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void TrackClickRecallButtonEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_RECALL_BUTTON_EVENT);
		}

		public void TrackClickAdventureButtonEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_ADVENTURE_BUTTON_EVENT);
		}
		public void TrackClickSwitchPetButtonEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_SWITCH_PET_BUTTON);
		}
		public void TrackSwitchPetEvent(string petId, string petUniqueId)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"pet_id", petId },
				{"pet_unique_id", petUniqueId },
			};
			_analytic.LogEvent(ANALYTIC_KEY.SWITCH_PET_EVENT, parameters);
		}
		public void TrackClickPetStatusButtonEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_STATUS_BUTTON);
		}
		public void TrackTouchPetEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.TOUCH_PET_EVENT);
		}
	}
}