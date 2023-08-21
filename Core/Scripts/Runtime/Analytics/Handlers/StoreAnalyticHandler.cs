using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class StoreAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{

			public const string CLICK_STORE_COINS_TAB_EVENT = "click_store_coins_tab";
			public const string CLICK_STORE_LOOTBOX_TAB_EVENT = "click_store_lootbox_tab";
			public const string CLICK_STORE_PETBOX_TAB_EVENT = "click_store_petbox_tab";

			public const string CLICK_STORE_COINS_ITEM_EVENT = "click_store_coins_item";
			public const string BUY_STORE_COINS_ITEM = "buy_store_coins_item";

			public const string CLICK_STORE_LOOTBOX_ITEM_INFORMATION_EVENT = "click_store_lootbox_item_information";
			public const string CLICK_STORE_LOOTBOX_ITEM_EVENT = "click_store_lootbox_item";
			public const string BUY_STORE_LOOTBOX_ITEM_EVENT = "buy_store_lootbox_item";

			public const string CLICK_BUY_PETBOX_ITEM_EVENT = "click_buy_petbox_item";
			public const string BUY_STORE_PETBOX_ITEM_EVENT = "buy_pet_box_item";
		}

		public StoreAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }
		#region TAB
		public void TrackClickStoreCoinsTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_COINS_TAB_EVENT);
		}

		public void TrackClickStoreLootboxTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_LOOTBOX_TAB_EVENT);
		}
		public void TrackClickStorePetboxTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_PETBOX_TAB_EVENT);
		}
		#endregion

		#region STORE COINS
		public void TrackClickStoreCoinsItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_COINS_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackBuyStoreCoinsItemEvent(BuyCoinsParameterData data)
		{
			if (data == null) return;

			var parameters = new Dictionary<string, object>()
			{
				{"item_id", data.ItemId },
				{"currency", data.Currency },
				{"cost", data.Cost },
				{"items", data.Items },
			};

			_analytic.LogEvent(ANALYTIC_KEY.BUY_STORE_COINS_ITEM, parameters);
		}
		#endregion

		#region STORE LOOTBOX
		public void TrackClickStoreLootboxItemInformationEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_LOOTBOX_ITEM_INFORMATION_EVENT, "item_id", itemId);
		}

		public void TrackClickStoreLootboxItemEvent(string itemId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_LOOTBOX_ITEM_EVENT, "item_id", itemId);
		}

		public void TrackBuyStoreLootboxItemEvent(BuyLootboxParameterData data)
		{
			if (data == null) return;

			var parameters = new Dictionary<string, object>()
			{
				{"item_id", data.ItemId },
				{"currency", data.Currency },
				{"cost", data.Cost },
				{"items", data.Items },
			};

			_analytic.LogEvent(ANALYTIC_KEY.BUY_STORE_LOOTBOX_ITEM_EVENT, parameters);
		}
		#endregion

		#region STORE PETBOX
		public void TrackClickBuyPetboxItemEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_BUY_PETBOX_ITEM_EVENT);
		}

		public void TrackBuyStorePetboxItemEvent(BuyPetboxParameterData data)
		{
			if (data == null) return;
			var parameters = new Dictionary<string, object>()
			{
				{"item_id", data.ItemId },
				{"currency", data.Currency },
				{"cost", data.Cost },
				{"pity_count", data.PityCount },
				{"items", data.Items },
			};
			_analytic.LogEvent(ANALYTIC_KEY.BUY_STORE_PETBOX_ITEM_EVENT, parameters);
		}
		#endregion

		public class BuyCoinsParameterData
		{
			public string ItemId { get; set; }
			public string Currency { get; set; }
			public decimal Cost { get; set; }
			public List<ItemData> Items { get; set; }
		}

		public class BuyLootboxParameterData
		{
			public string ItemId { get; set; }
			public string Currency { get; set; }
			public decimal Cost { get; set; }
			public List<PinParameterData> Items { get; set; }

			public BuyLootboxParameterData(string itemId,  string currency, decimal cost, List<PinParameterData> items)
			{
				ItemId = itemId;
				Currency = currency;
				Cost = cost;
				Items = items;
			}
		}
		public class BuyPetboxParameterData
		{
			public string ItemId { get; set; }
			public string Currency { get; set; }
			public decimal Cost { get; set; }
			public int PityCount { get; set; }
			public List<PetboxResultParameterData> Items { get; set; }

			public BuyPetboxParameterData(string itemId,string currency, decimal cost, int pityCount, List<PetboxResultParameterData> items)
			{
				ItemId = itemId; 
				Currency = currency; 
				Cost = cost; 
				PityCount = pityCount; 
				Items = items;
			}
		}

		public class PinParameterData
		{
			public string Id { get; set; }
			public int Rarity { get; set; }

			public PinParameterData(string id, int rarity) { Id = id; Rarity = rarity; }
		}

		public class ItemData
		{
			public string Type { get; set; }
			public decimal Amount { get; set; }
		}

		public class PetboxResultParameterData
		{
			public string Type { get; set; }
			public string Id { get; set; }
			public int Amount { get; set; }
		}
	}
}

