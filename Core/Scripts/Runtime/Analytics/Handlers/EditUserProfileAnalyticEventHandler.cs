using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class EditUserProfileAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_EDIT_PROFILE_AVATAR_TAB_EVENT = "click_edit_profile_avatar_tab";
			public const string CLICK_EDIT_PROFILE_AVATAR_ITEM_EVENT = "click_edit_profile_avatar_item";
			public const string CLICK_EDIT_PROFILE_FRAME_TAB_EVENT = "click_edit_profile_frame_tab";
			public const string CLICK_EDIT_PROFILE_FRAME_ITEM_EVENT = "click_edit_profile_frame_item";

			public const string SET_AVATAR_ITEM_EVENT = "set_avatar_item";
			public const string SET_FRAME_ITEM_EVENT = "set_frame_item";
			public const string SAVE_EDIT_PROFILE = "save_edit_profile";

			public const string BUY_AVATAR_ITEM_EVENT = "buy_avatar_item";
			public const string BUY_FRAME_ITEM_EVENT = "buy_frame_item";
		}

		public EditUserProfileAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void TrackSetAvatarItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.SET_AVATAR_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackSetFrameItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.SET_FRAME_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackSaveEditProfileEvent(string avatarId, string frameId)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"avatar_id", avatarId },
				{"frame_id", frameId },
			};
			_analytic.LogEvent(ANALYTIC_KEY.SAVE_EDIT_PROFILE, parameters);
		}

		public void TrackClickEditProfileAvatarTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_EDIT_PROFILE_AVATAR_TAB_EVENT);
		}

		public void TrackEditEditProfileAvatarItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_EDIT_PROFILE_AVATAR_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackClickEditProfileFrameTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_EDIT_PROFILE_FRAME_TAB_EVENT);
		}

		public void TrackClickEditProfileFrameItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_EDIT_PROFILE_FRAME_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackBuyAvatarEvent(BuyAccessoriesParameters data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"item_id", data.ItemId },
				{"currency", data.Currency },
				{"cost", data.Cost },
			};
			_analytic.LogEvent(ANALYTIC_KEY.BUY_AVATAR_ITEM_EVENT, parameters);
		}

		public void TrackBuyFrameEvent(BuyAccessoriesParameters data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"item_id", data.ItemId },
				{"currency", data.Currency },
				{"cost", data.Cost },
			};
			_analytic.LogEvent(ANALYTIC_KEY.BUY_FRAME_ITEM_EVENT, parameters);
		}

		public class BuyAccessoriesParameters
		{
			public string ItemId { get; set; }
			public string Currency { get; set; }
			public decimal Cost { get; set; }
		}

	}
}
