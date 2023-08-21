using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class UserProfileAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_EDIT_PROFILE_EVENT = "click_edit_profile";
			public const string CLICK_CLOSE_USER_PROFILE_EVENT = "click_close_user_profile";
			public const string CLICK_MY_PROFILE_TAB_MENU_EVENT = "click_my_profile_tab_menu";
			public const string CLICK_COLLECTIBLE_TAB_MENU_EVENT = "click_collectible_tab_menu";
			public const string CLICK_FRIENDLIST_TAB_MENU_EVENT = "click_friendslist_tab_menu";
			public const string CLICK_STATISTIC_TAB_MENU_EVENT = "click_statistic_tab_menu";


		}

		public UserProfileAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickEditProfiileEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_EDIT_PROFILE_EVENT);
		}

		public void TrackClickCloseUserProfiileEvent(string tab)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_CLOSE_USER_PROFILE_EVENT, "tab_location", tab);
		}

		public void TrackClickMyProfileTabMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MY_PROFILE_TAB_MENU_EVENT);
		}

		public void TrackClickCollectibleTabMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_TAB_MENU_EVENT);
		}

		public void TrackClickFriendlistTabMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_FRIENDLIST_TAB_MENU_EVENT);
		}

		public void TrackClickStatisticTabMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STATISTIC_TAB_MENU_EVENT);
		}
	}
}
