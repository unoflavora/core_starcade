using Agate.Starcade.Core.Runtime.Analytics.Controllers;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class FriendAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_FRIENDLIST_TAB_EVENT = "click_friendlist_tab";
			public const string CLICK_FRIENDLIST_SEARCH_TAB_EVENT = "click_friendlist_search_tab";
			public const string CLICK_FRIENDLIST_REQUEST_TAB_EVENT = "click_friendlist_request_tab";

			public const string CLICK_GIVE_FRIEND_COLLECTIBLE_PIN_EVENT = "click_give_friend_collectible_pin";
			public const string SEARCH_FRIEND_EVENT = "search_friend";
			public const string ADD_FRIEND_EVENT = "add_friend";
			public const string COPY_FRIENDCODE_EVENT = "copy_friendcode";
			public const string ACCEPT_FRIEND_REQUEST_EVENT = "accept_friend_request";
			public const string DECLINE_FRIEND_REQUEST_EVENT = "decline_friend_request";
			public const string UNFRIEND_EVENT = "unfriend";

		}

		public FriendAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickFriendlistTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_FRIENDLIST_TAB_EVENT);
		}

		public void TrackClickFriendlistSearchTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_FRIENDLIST_SEARCH_TAB_EVENT);
		}

		public void TrackClickFriendlistRequestTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_FRIENDLIST_REQUEST_TAB_EVENT);
		}


		public void TrackClickGiveFriendCollectiblePinEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_GIVE_FRIEND_COLLECTIBLE_PIN_EVENT);
		}

		public void TrackSearchFriendEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.SEARCH_FRIEND_EVENT);
		}
		public void TrackAddFriendEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.ADD_FRIEND_EVENT);
		}
		public void TrackCopyFriendcodeEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.COPY_FRIENDCODE_EVENT);
		}
		public void TrackAcceptFriendRequestEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.ACCEPT_FRIEND_REQUEST_EVENT);
		}
		public void TrackDeclineFriendRequestEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.DECLINE_FRIEND_REQUEST_EVENT);
		}
		public void TrackUnfriendEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.UNFRIEND_EVENT);
		}
	}
}
