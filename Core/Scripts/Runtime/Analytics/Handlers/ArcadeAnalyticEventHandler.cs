using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
	public class ArcadeAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string GAME_ID_PARAM = "game_id";
			public const string GAME_MODE_PARAM = "game_mode";
			public const string SESSION_ID_PARAM = "session_id";

			public const string SESSION_START_EVENT = "session_start";
			public const string SESSION_END_EVENT = "session_end";

			public const string TIME_OUT_EVENT = "time_out";
			public const string CHANGE_MODE_EVENT = "change_mode";

			public const string INSUFFICIENT_BALANCE_EVENT = "insufficient_balance";

			public const string CLICK_USER_PROFILE = "click_arcade_user_profile";
			public const string CLICK_GOTO_STORE = "click_arcade_goto_store";
		}

		private string _gameId { get; set; } = "";
		private string _gameMode { get; set; } = "";
		private string _sessionId { get; set; } = "";


		public ArcadeAnalyticEventHandler(IAnalyticController analytic, string gameId, string gameMode, bool isEnabled = true) : base(analytic, isEnabled)
		{
			_gameId = gameId;
			_gameMode = gameMode;

		}

		public virtual void TrackSessionStartEvent(string sessionId)
		{
			_sessionId = sessionId;
		}

		public void TrackEndSessionEvent()
		{

			var parameters = CreateBasicArcadeParameters();

			LogEvent(ANALYTIC_KEY.SESSION_END_EVENT, parameters);
		}

		public void TrackTimeoutEvent()
		{
			var parameters = CreateBasicArcadeParameters();

			LogEvent(ANALYTIC_KEY.TIME_OUT_EVENT, parameters);
		}

		public void TrackChangeModeEvent(string mode)
		{
			LogEvent(ANALYTIC_KEY.CHANGE_MODE_EVENT, ANALYTIC_KEY.GAME_MODE_PARAM, mode);
		}


		public void TrackInsufficientBalanceEvent()
		{
			var parameters = CreateBasicArcadeParameters();
			LogEvent(ANALYTIC_KEY.INSUFFICIENT_BALANCE_EVENT, parameters);
		}

		public void TrackClickUserProfile()
		{
			var parameters = CreateBasicArcadeParameters();
			LogEvent(ANALYTIC_KEY.CLICK_USER_PROFILE, parameters);
		}
		public void TrackClickGotoStore ()
		{
			var parameters = CreateBasicArcadeParameters();
			LogEvent(ANALYTIC_KEY.CLICK_GOTO_STORE, parameters);
		}

		protected Dictionary<string, object> CreateBasicArcadeParameters()
		{
			return new Dictionary<string, object>()
			{
				{ANALYTIC_KEY.GAME_ID_PARAM, _gameId },
				{ANALYTIC_KEY.GAME_MODE_PARAM, _gameMode },
				{ANALYTIC_KEY.SESSION_ID_PARAM, _sessionId },
			};
		}
	}
}