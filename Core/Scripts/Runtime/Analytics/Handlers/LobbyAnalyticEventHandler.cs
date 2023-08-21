using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Firebase.Analytics;
using System;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class LobbyAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string LOBBY_EVENT = "lobby_open";

			public const string CLAIM_LOBBY_ONBOARDING_REWARD_EVENT = "claim_lobby_onboarding_reward";
			public const string CLAIM_TIME_BASED_REWARD_EVENT = "claim_time_based_coin_reward";

			public const string CLICK_ARCADE_MENU_EVENT = "click_arcade_menu";
			public const string CLICK_STORE_MENU_EVENT = "click_store_menu";
			public const string CLICK_POSTER_MENU_EVENT = "click_poster_menu";
			public const string CLICK_SETTING_MENU_EVENT = "click_setting_menu";
			public const string CLICK_NFT_COLLECTION_EVENT = "click_nft_collection";
			public const string CLICK_PET_MENU_EVENT = "click_pet_menu";
			public const string CLICK_USER_PROFILE_EVENT = "click_user_profile";
			public const string CLICK_MAILBOX_MENU_EVENT = "click_mailbox_menu";

			public const string CLICK_ARCADE_SELECTION_EVENT = "arcade_select";	
			public const string ARCADE_DOWNLOAD_EVENT = "arcade_download";
			public const string ARCADE_DOWNLOAD_SUCCESS_EVENT = "arcade_download_success";
			public const string CLICK_ARCADE_PLAY_EVENT = "click_arcade_play";

			public const string GAME_ID_PARAM = "game_id";
			public const string GAME_MODE_PARAM = "game_mode";

			public const string BIND_ACCOUNT_EVENT = "bind_account";
			public const string BIND_ACCOUNT_TYPE_EVENT = "account_type";
		}

		public LobbyAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		#region LobbyEvent
		public void TrackLobbyEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.LOBBY_EVENT);
		}

		public void TrackClaimTimeBasedCoinRewardEvent(string rewardType, decimal rewardAmount)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"reward_type", rewardType },
				{"reward_amount", rewardAmount },
			};

			_analytic.LogEvent(ANALYTIC_KEY.CLAIM_TIME_BASED_REWARD_EVENT, parameters);
		}


		public void TrackClaimLobbyOnboardingRewardEvent(List<RewardParameters> data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"reward_object", data },
			};
			for(int i=0;i<data.Count; i++)
			{
				var reward = data[i];
				parameters.Add($"reward_{i + 1}_type", reward.Type);
				parameters.Add($"reward_{i + 1}_amount", reward.Amount);
			}

			_analytic.LogEvent(ANALYTIC_KEY.CLAIM_LOBBY_ONBOARDING_REWARD_EVENT, parameters);
		}
		#endregion

			#region Menu
			public void TrackClickArcadeMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_ARCADE_MENU_EVENT);
		}

		public void TrackClickStoreMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_STORE_MENU_EVENT);
		}

		public void TrackClickPosterMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_POSTER_MENU_EVENT);
		}
		public void TrackClickNFTCollectionMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_NFT_COLLECTION_EVENT);
		}

		public void TrackClickPetMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_MENU_EVENT);
		}

		public void TrackClickMailboxMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MAILBOX_MENU_EVENT);
		}

		public void TrackClickUserProfileEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_USER_PROFILE_EVENT);
		}

		public void TrackClickSettingMenuEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_SETTING_MENU_EVENT);
		}
		#endregion

		#region ArcadeSelection
		public void TrackClickArcadePlayEvent(string gameId, string gameMode)
		{
			var parameters = new Dictionary<string, object>()
			{
				{ANALYTIC_KEY.GAME_ID_PARAM, gameId },
				{ANALYTIC_KEY.GAME_MODE_PARAM, gameMode },
			};

			_analytic.LogEvent(ANALYTIC_KEY.CLICK_ARCADE_PLAY_EVENT, parameters);
		}
		public void TrackClickSelectArcadeEvent(string gameId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_ARCADE_SELECTION_EVENT, ANALYTIC_KEY.GAME_ID_PARAM, gameId);
		}
		public void TrackDownloadArcade(string gameId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.ARCADE_DOWNLOAD_EVENT, ANALYTIC_KEY.GAME_ID_PARAM, gameId);
		}
		public void TrackArcadeDownloadSuccess(string gameId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.ARCADE_DOWNLOAD_SUCCESS_EVENT, ANALYTIC_KEY.GAME_ID_PARAM, gameId);
		}

		#endregion

		public class RewardParameters
		{
			public string Type { get; set; }
			public float Amount { get; set; }
		}
	}
}
