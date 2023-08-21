using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Firebase.Analytics;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class TitleAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string FTUE_TITLE = "ftue_title";
			public const string TITLE_OPEN = "title_open";

			public const string CLICK_LOGIN = "click_login";
			public const string LOGIN_SUCCESS = "login_success";

			public const string TAP_TO_PLAY_EVENT = "tap_to_play";

			public const string LOGIN_TYPE_PARAM = "login_type";
		}

		public static string PLAYER_PREF_FTUE_ANALYTIC = $"analytic_{ANALYTIC_KEY.FTUE_TITLE}";

		public TitleAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackOpenTitleScreenEvent()
		{
			var IsFTUETitleCalled = PlayerPrefs.GetInt(PLAYER_PREF_FTUE_ANALYTIC, 0) == 1;
			Debug.Log("IsFTUETitleCalled" + IsFTUETitleCalled);

			if (!IsFTUETitleCalled)
			{
				_analytic.LogEvent(ANALYTIC_KEY.FTUE_TITLE);
				PlayerPrefs.SetInt(PLAYER_PREF_FTUE_ANALYTIC, 1);
			}
			_analytic.LogEvent(ANALYTIC_KEY.TITLE_OPEN);
		}

		public void TrackLoginEvent(string loginType)
		{
			_analytic.LogEvent(FirebaseAnalytics.EventLogin);
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_LOGIN + "_" + loginType);
		}

		public void TrackLoginSuccess(string loginType)
		{
			_analytic.LogEvent(FirebaseAnalytics.EventLogin);
			_analytic.LogEvent(ANALYTIC_KEY.LOGIN_SUCCESS, ANALYTIC_KEY.LOGIN_TYPE_PARAM, loginType);
		}

		public void TrackTapToPlayEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.TAP_TO_PLAY_EVENT);
		}

	}
}
