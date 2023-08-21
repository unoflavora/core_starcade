using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class CollectibleAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_COLLECTIBLE_ALBUM_EVENT = "click_collectible_album";
			public const string CLAIM_COLLECTIBLE_ALBUM_REWARD_EVENT = "claim_collectible_album_reward";
			public const string CLICK_SHARE_COLLECTIBLE_ALBUM_EVENT = "click_share_collectible_album";
			public const string CLICK_COLLECTIBLE_ALBUM_BACK_BUTTON_EVENT = "click_collectible_album_back_button";
			public const string CLICK_CONVERT_COLLECTIBLE_ALBUM_ITEMS_EVENT = "click_convert_collectible_album_items";
			public const string CONVERT_COLLECTIBLE_ALBUM_ITEMS_SUCCESS_EVENT = "convert_collectible_album_items_success";
			public const string CLICK_COLLECTIBLE_ALBUM_ITEM_EVENT = "click_collectible_album_item";
			public const string CLICK_COLLECTIBLE_ALBUM_ITEM_BUY_BUTTON_EVENT = "click_collectible_album_item_buy_button";
			public const string CLICK_COLLECTIBLE_ALBUM_ITEM_SEND_BUTTON_EVENT = "click_collectible_album_item_send_button";


		}

		public CollectibleAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		
		public void TrackClickCollectibleAlbumEvent(string setId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_ALBUM_EVENT, "set_id", setId);
		}

		public void TrackClaimCollectibleAlbumRewardEvent(string setId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLAIM_COLLECTIBLE_ALBUM_REWARD_EVENT, "set_id", setId);
		}
		public void TrackClickShareCollectibleAlbumEvent(string setId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_SHARE_COLLECTIBLE_ALBUM_EVENT, "set_id", setId);
		}
		public void TrackClickCollectibleAlbumBackButtonEvent(string setId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_ALBUM_BACK_BUTTON_EVENT, "set_id", setId);
		}

		public void TrackClickConvertCollectibleAlbumItemsEvent(string setId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_CONVERT_COLLECTIBLE_ALBUM_ITEMS_EVENT, "set_id", setId);
		}

		public void TrackConvertCollectibleAlbumItemsSuccessEvent(ConvertCollectiblePinParameterData data)
		{
			if (data == null) return;

			var parameters = new Dictionary<string, object>()
			{
				{"set_id", data.SetId },
				{"result_pin_id", data.ResultPinId },
				{"result_pin_rarity", data.ResultPinRarity },
				{"pins", data.Pins },
			};
			_analytic.LogEvent(ANALYTIC_KEY.CONVERT_COLLECTIBLE_ALBUM_ITEMS_SUCCESS_EVENT, parameters);
		}

		public void TrackClickCollectibleAlbumItemEvent(string setId, string itemId)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"set_id", setId },
				{"item_id", itemId },
			};

			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_ALBUM_ITEM_EVENT, parameters);
		}

		public void TrackCollectibleAlbumItemBuyButtonEvent(string setId, string itemId)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"set_id", setId },
				{"item_id", itemId },
			};

			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_ALBUM_ITEM_BUY_BUTTON_EVENT, parameters);
		}

		public void TrackCollectibleAlbumItemSendButtonEvent(string setId, string itemId)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"set_id", setId },
				{"item_id", itemId },
			};

			_analytic.LogEvent(ANALYTIC_KEY.CLICK_COLLECTIBLE_ALBUM_ITEM_SEND_BUTTON_EVENT, parameters);
		}

		public class ConvertCollectiblePinParameterData
		{
			public string SetId { get; set; }
			public string ResultPinId { get; set; }
			public int ResultPinRarity{ get; set; }
			public List<PinParameterData> Pins { get; set; }
		}

		public class PinParameterData
		{
			public string Id { get; set; }
			public int Rarity { get; set; }
		}
	}
}
