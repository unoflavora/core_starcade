using Agate.Starcade.Core.Runtime.Analytics;
using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Spine;

namespace Starcade.Core.Runtime.Analytics.Handlers
{
    public class MonstamatchAnalyticEventHandler : ArcadeAnalyticEventHandler
    {
		public static class MONSTAMATCH_ANALYTIC_KEY
		{
			public const string START_EVENT = "mt02_start";
			public const string MATCH_EVENT = "mt02_match";

			public const string SPECIAL_JACKPOT_RECEIVE = "mt02_special_jackpot_receive";

			public const string PUZZLE_JACKPOT_RECEIVE_PIECE = "mt02_puzzle_jackpot_receive_piece";
			public const string PUZZLE_JACKPOT_COMPLETE = "mt02_puzzle_jackpot_complete";

			public const string ROULETTE_JACKPOT_RECEIVE = "mt02_roulette_jackpot_receive";
			public const string ROULETTE_JACKPOT_COMPLETE = "mt02_roulette_jackpot_complete";

			public const string MONSTER_JACKPOT_START = "mt02_monster_jackpot_start";
			public const string MONSTER_JACKPOT_SHOOT = "mt02_monster_jackpot_shoot";
			public const string MONSTER_JACKPOT_COMPLETE = "mt02_monster_jackpot_complete";

			public const string MONSTER_JACKPOT_BUY_AMMO = "mt02_monster_jackpot_buy_ammo";
		}

		public MonstamatchAnalyticEventHandler(IAnalyticController analytic, string gameMode, bool isEnabled = true) : base(analytic, StarcadeConstants.ARCADE_CODE_NAME_MONSTAMATCH, gameMode, isEnabled)
		{

        }

		public virtual void TrackStartSessionEvent(string sessionId)
		{
			base.TrackSessionStartEvent(sessionId);

			var parameters = CreateBasicArcadeParameters();

			LogEvent(ANALYTIC_KEY.SESSION_START_EVENT, parameters);
		}

		public virtual void TrackStartEvent(StartEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("spin_session_id", eventData.CostCurrency);
			parameters.Add("cost_currency", eventData.CostCurrency);
			parameters.Add("cost", eventData.Cost);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.START_EVENT, parameters);
		}

		public virtual void TrackMatchEvent(MatchEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("spin_session_id", eventData.SpinSessionId);
			parameters.Add("reward_currency", eventData.RewardCurrency);
			parameters.Add("total_reward", eventData.TotalReward);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.MATCH_EVENT, parameters);
		}

		public virtual void TrackReceiveSpecialJackpot(SpecialJackpotReceiveEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("type", eventData.RewardType);
			parameters.Add("reward_currency", eventData.RewardCurrency);
			parameters.Add("total_reward", eventData.TotalReward);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.SPECIAL_JACKPOT_RECEIVE, parameters);
		}

		public virtual void TrackReceivePuzzlePiece()
		{
			var parameters = CreateBasicArcadeParameters();

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.PUZZLE_JACKPOT_RECEIVE_PIECE, parameters);
		}

		public virtual void TrackCompletePuzzleJackpot(PuzzleJackpotReceiveEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("reward_currency", eventData.RewardCurrency);
			parameters.Add("total_reward", eventData.TotalReward);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.PUZZLE_JACKPOT_COMPLETE, parameters);
		}

		public virtual void TrackReceiveRouletteJackpot()
		{
			var parameters = CreateBasicArcadeParameters();

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.ROULETTE_JACKPOT_RECEIVE, parameters);
		}


		public virtual void TrackCompleteRouletteJackpot(RouletteJackpotCompleteEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("reward_tier", eventData.RewardTier);
			parameters.Add("reward_type", eventData.RewardType);
			parameters.Add("total_reward", eventData.TotalReward);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.ROULETTE_JACKPOT_COMPLETE, parameters);
		}


		public virtual void TrackStartMonsterJackpot()
		{
			var parameters = CreateBasicArcadeParameters();

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.MONSTER_JACKPOT_START, parameters);
		}

		public virtual void TrackShootMonsterJackpot(string target)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("target", target);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.MONSTER_JACKPOT_SHOOT, parameters);
		}

		public virtual void TrackCompleteMonsterJackpot(MonsterJackpotCompleteEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("reward_tier", eventData.RewardTier);
			parameters.Add("reward_currency", eventData.RewardCurrency);
			parameters.Add("total_reward", eventData.TotalReward);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.MONSTER_JACKPOT_COMPLETE, parameters);
		}

		public virtual void TrackBuyAmmoOnMonsterJackpot(MonsterJackpotBuyAmmoEventData eventData)
		{
			var parameters = CreateBasicArcadeParameters();
			parameters.Add("item_id", eventData.ItemId);
			parameters.Add("cost_currency", eventData.CostCurrency);
			parameters.Add("cost", eventData.Cost);

			LogEvent(MONSTAMATCH_ANALYTIC_KEY.MONSTER_JACKPOT_BUY_AMMO, parameters);
		}

		public class StartEventData
		{
			public string SpinSessionId { get; set; }
			public string CostCurrency { get; set; }
			public double Cost { get; set; }
		}


		public class MatchEventData
		{
			public string SpinSessionId { get; set; }
			public string RewardCurrency { get; set; } 
			public double TotalReward { get; set; } 
		}

		public class SpecialJackpotReceiveEventData
		{
			public string RewardType { get; set; }
			public string RewardCurrency { get; set; }
			public double TotalReward { get; set; }
		}

		public class PuzzleJackpotReceiveEventData
		{
			public string RewardCurrency { get; set; }
			public double TotalReward { get; set; }
		}

		public class RouletteJackpotCompleteEventData
		{
			public int RewardTier { get; set; }
			public string RewardType { get; set; }
			public double TotalReward { get; set; }
		}

		public class MonsterJackpotCompleteEventData
		{
			public int RewardTier { get; set; }
			public string RewardCurrency { get; set; }
			public double TotalReward { get; set; }
		}
		public class MonsterJackpotBuyAmmoEventData
		{
			public string ItemId { get; set; }
			public string CostCurrency { get; set; }
			public double Cost { get; set; }
		}
	}
}
