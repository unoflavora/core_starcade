using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class IngameOptionAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string GAME_ID_PARAM = "game_id";
			public const string INGAME_OPTION_OPEN_EVENT = "ingame_option_open";

			public const string CLICK_INGAME_OPTION_SETTING_EVENT = "ingame_option_click_setting";
			public const string CLICK_INGAME_OPTION_TOOLTIPS_EVENT = "ingame_option_click_tooltips";
			public const string CLICK_INGAME_OPTION_QUIT_GAME_EVENT = "ingame_option_click_quit";

		}

		private string _gameId { get; set; } = "";

		public IngameOptionAnalyticEventHandler
			(IAnalyticController analytic, string gameId) : base(analytic)
        {
			_gameId = gameId;
        }

		public void TrackOpenEvent()
		{
			var parameters = CreateBasicArcadeParameters();
			_analytic.LogEvent(ANALYTIC_KEY.INGAME_OPTION_OPEN_EVENT, parameters);
		}

		public void TrackClickGameSettingEvent()
		{
			var parameters = CreateBasicArcadeParameters();
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_INGAME_OPTION_SETTING_EVENT, parameters);
		}

		public void TrackClickTooltipsEvent()
		{
			var parameters = CreateBasicArcadeParameters();
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_INGAME_OPTION_TOOLTIPS_EVENT, parameters);
		}

		public void TrackClickQuitEvent()
		{
			var parameters = CreateBasicArcadeParameters();
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_INGAME_OPTION_QUIT_GAME_EVENT, parameters);
		}

		protected Dictionary<string, object> CreateBasicArcadeParameters()
		{
			return new Dictionary<string, object>()
			{
				{ANALYTIC_KEY.GAME_ID_PARAM, _gameId },
			};
		}
	}
}
