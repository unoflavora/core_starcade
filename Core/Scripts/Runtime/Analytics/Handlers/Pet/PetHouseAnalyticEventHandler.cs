using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class PetHouseAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string SELECT_PET_HOUSE_FILTER_EVENT = "select_pet_house_filter";
			public const string CLICK_PET_HOUSE_ITEM_EVENT = "click_pet_house_item";
			public const string CLICK_PET_HOUSE_ITEM_GIVE_PET_BUTTON_EVENT = "click_pet_house_item_give_pet_button";
			public const string CLICK_PET_HOUSE_ITEM_LEVEL_UP_BUTTON_EVENT = "click_pet_house_item_level_up_button";

			public const string GIVE_PET_EVENT = "give_pet";
			public const string LEVEL_UP_EVENT = "level_up_pet";
			public const string TOUCH_PET_AT_PET_HOUSE_EVENT = "touch_pet_at_pet_house";
		}

		public PetHouseAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void SelectPetHouseFilterEvent(string type)
		{
			_analytic.LogEvent(ANALYTIC_KEY.SELECT_PET_HOUSE_FILTER_EVENT, "type", type);
		}

		public void TrackClickPetHouseItemEvent(string petId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_HOUSE_ITEM_EVENT, "pet_id", petId);
		}

		public void TrackClickPetHouseItemGivePetButtonEvent(string petId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_HOUSE_ITEM_GIVE_PET_BUTTON_EVENT, "pet_id", petId);
		}

		public void TrackClickPetHouseItemLevelUpButtonEvent(string petId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_HOUSE_ITEM_LEVEL_UP_BUTTON_EVENT, "pet_id", petId);
		}

		public void TrackGivePetEvent(GivePetParameterData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"pet_id", data.PetId },
				{"pet_unique_id", data.PetUniqueId },
				{"pet_level", data.PetLevel },
				{"sub_skills", data.SubSkills },
			};
			_analytic.LogEvent(ANALYTIC_KEY.GIVE_PET_EVENT, parameters);
		}

		public void TrackLevelUpPetEvent(LevelUpPetParameterData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"pet_id", data.PetId },
				{"pet_unique_id", data.PetUniqueId },
				{"current_level", data.CurrentLevel },
				{"target_level", data.TargetLevel },
				{"pins", data.Pins },
			};
			_analytic.LogEvent(ANALYTIC_KEY.LEVEL_UP_EVENT, parameters);
		}

		public void TrackTouchPetAtPetHouseEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.TOUCH_PET_AT_PET_HOUSE_EVENT);
		}

		public class GivePetParameterData
		{
			public string PetId { get; set; }
			public string PetUniqueId { get; set; }
			public int PetLevel { get; set; }
			public List<SkillParameterData> SubSkills { get; set; }
		}

		public class LevelUpPetParameterData
		{
			public string PetId { get; set; }
			public string PetUniqueId { get; set; }
			public int CurrentLevel { get; set; }
			public int TargetLevel { get; set; }
			public List<PinParameterData> Pins { get; set; }
		}

		public class SkillParameterData
		{
			public string Id { get; set; }
			public float Value { get; set; }
		}

		public class PinParameterData
		{
			public string Id { get; set; }
			public int Rarity { get; set; }
		}
	}
}