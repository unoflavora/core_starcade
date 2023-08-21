using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers
{
    public class UserAnalyticEventHandler : BaseAnalyticEventHandler
    {
		public static class ANALYTIC_KEY
		{
			public const string REGION = "region";

			//USER
			public const string USER_ID_PARAM = "player_id";
			public const string USER_FRIENDCODE_PARAM = "friendcode";
			public const string USER_EMAIL_PARAM = "email";
			public const string USER_DISPLAY_NAME_PARAM = "display_name";
			public const string USER_LEVEL_PARAM = "level";
			public const string USER_ACCOUNT_TYPE_PARAM = "account_type";

			//EVENT
			public const string USER_LEVEL_UP_EVENT = "level_up";

			//BALANCE
			public const string CURRENT_STAR_COIN_PARAM = "current_gold";
			public const string CURRENT_GOLD_COIN_PARAM = "current_star";
			public const string CURRENT_STAR_TICKET_PARAM = "current_star_ticket";

			public const string SPEND_STAR_COIN_PARAM = "gold_spend";
			public const string SPEND_GOLD_COIN_PARAM = "star_spend";
			public const string SPEND_STAR_TICKET_PARAM = "star_ticket_spend";
		}

		private string _userId { get; set; } = "";

		public UserAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
        {

        }

		public void SetUserId(string userId)
		{
			// Log an event with no parameters.
			_userId = userId;
			_analytic.SetUserId(userId);

			_analytic.SetUserProperty(ANALYTIC_KEY.USER_ID_PARAM, userId);
		}

		public void TrackFriendcodeProperties(long friendcode)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_FRIENDCODE_PARAM, friendcode.ToString());
		}

		public void TrackUserLevelProperties(int level)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_LEVEL_PARAM, level.ToString());
		}

		public void TrackUserLevelUpEvent(int level)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_LEVEL_UP_EVENT, level.ToString());
		}

		public void TrackUserEmailProperties(string email)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_EMAIL_PARAM, email);
		}

		public void TrackUserDisplayNameProperties(string name)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_DISPLAY_NAME_PARAM, name);
		}

		public void TrackAccountTypeProperties(string accountType)
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.USER_ACCOUNT_TYPE_PARAM, accountType);
		}

		public void TrackRegionProperties()
		{
			_analytic.SetUserProperty(ANALYTIC_KEY.REGION, System.Globalization.RegionInfo.CurrentRegion.ToString());
		}

		public void TrackUserBalanceProperties(PlayerBalance userBalance)
        {
			TrackUserBalance(CurrencyTypeEnum.GoldCoin, userBalance.GoldCoin);
			TrackUserBalance(CurrencyTypeEnum.StarCoin, userBalance.StarCoin);
			TrackUserBalance(CurrencyTypeEnum.StarTicket, userBalance.StarTicket);

        }

		public void TrackUserBalance(CurrencyTypeEnum currencyType, double amount)
		{

			switch (currencyType)
			{
				case CurrencyTypeEnum.GoldCoin:
					_analytic.SetUserProperty(ANALYTIC_KEY.CURRENT_GOLD_COIN_PARAM, amount.ToString());
					break;
				case CurrencyTypeEnum.StarCoin:
					_analytic.SetUserProperty(ANALYTIC_KEY.CURRENT_STAR_COIN_PARAM, amount.ToString());
					break;
				case CurrencyTypeEnum.StarTicket:
					_analytic.SetUserProperty(ANALYTIC_KEY.CURRENT_STAR_TICKET_PARAM, amount.ToString());
					break;
			}
		}
	}
}
