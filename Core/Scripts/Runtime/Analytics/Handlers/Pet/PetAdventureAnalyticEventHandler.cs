using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class PetAdventureAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string DISPATCH_PET_ADVENTURE_EVENT = "dispatch_pet_adventure";
			public const string RECALL_PET_ADVENTURE_EVENT = "recall_pet_adventure";
			public const string CLAIM_COMPLETE_PET_ADVENTURE_EVENT = "claim_complete_pet_adventure";

		}

		public PetAdventureAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void TrackDispatchPetAdventureEvent(DispatchPetData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"session_id", data.SessionId },
				{"configId", data.ConfigId },
				{"pet_id", data.PetId },
				{"pet_unique_id", data.PetUniqueId },
				{"pet_level", data.PetLevel },
				{"basic_skill", data.BasicSkill },
				{"sub_skills", data.SubSkills },
			};
			_analytic.LogEvent(ANALYTIC_KEY.DISPATCH_PET_ADVENTURE_EVENT, parameters);
		}

		public void TrackRecallPetAdventureEvent(BaseAdventureData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"session_id", data.SessionId },
				{"configId", data.ConfigId },
				{"pet_id", data.PetId },
				{"pet_unique_id", data.PetUniqueId },
			};
			_analytic.LogEvent(ANALYTIC_KEY.RECALL_PET_ADVENTURE_EVENT, parameters);
		}

		public void TrackClaimCompletePetAdventureEvent(ClaimCompleteAdventureData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"session_id", data.SessionId },
				{"configId", data.ConfigId },
				{"pet_id", data.PetId },
				{"pet_unique_id", data.PetUniqueId },
				{"pet_level", data.PetLevel },
				{"reward_object", data.Rewards },
			};

			for (int i = 0; i < data.Rewards.Count; i++)
			{
				var reward = data.Rewards[i];
				parameters.Add($"reward_{i + 1}_type", reward.Type);
				parameters.Add($"reward_{i + 1}_amount", reward.Amount);
			}

			_analytic.LogEvent(ANALYTIC_KEY.CLAIM_COMPLETE_PET_ADVENTURE_EVENT, parameters);
		}

		public class BaseAdventureData
		{
			public string SessionId { get; set; }
			public string ConfigId { get; set; }
			public string PetId { get; set; }
			public string PetUniqueId { get; set; }
		}

		public class DispatchPetData: BaseAdventureData
		{
			public int PetLevel { get; set; }
			public SkillData BasicSkill { get; set; }
			public List<SkillData> SubSkills { get; set; }
		}

		public class ClaimCompleteAdventureData : BaseAdventureData
		{
			public string PetLevel { get; set; }
			public List<RewardData> Rewards { get; set; }
		}

		public class SkillData
		{
			public string Id { get; set;}
			public float Value { get; set; }
		}

		public class RewardData
		{
			public string Type { get; set; }
			public float Amount { get; set; }
		}
	}
}