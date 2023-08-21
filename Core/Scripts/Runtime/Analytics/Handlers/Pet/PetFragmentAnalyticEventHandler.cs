using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class PetFragmentAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string SELECT_PET_FRAGMENT_FILTER_EVENT = "select_pet_fragment_filter";
			public const string CLICK_PET_FRAGMENT_ITEM_EVENT = "click_pet_fragment_item";
			public const string CLICK_PET_FRAGMENT_GOTO_ADVENTURE_BOX_EVENT = "click_pet_fragment_goto_adventure_box";
			public const string CLICK_PET_FRAGMENT_GOTO_STORE_EVENT = "click_pet_fragment_goto_store";
			public const string COMBINE_PET_FRAGMENT_EVENT = "combine_pet_fragments";
		}

		public PetFragmentAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void SelectPetFragmentFilterEvent(string type)
		{
			_analytic.LogEvent(ANALYTIC_KEY.SELECT_PET_FRAGMENT_FILTER_EVENT, "type", type);
		}

		public void TrackClickPetFragmentItemEvent(SelectPetFragmentParameterData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"pet_id", data.PetId },
				{"is_combineable", data.IsCombineable },
			};
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_FRAGMENT_ITEM_EVENT, parameters);
		}

		public void TrackClickPetFragmentGotoAdventureBoxEvent(string petId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_FRAGMENT_GOTO_ADVENTURE_BOX_EVENT, "pet_id", petId);
		}

		public void TrackClickPetFragmentGotoStoreEvent(string petId)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_FRAGMENT_GOTO_STORE_EVENT, "pet_id", petId);
		}

		public void TrackCombinePetFragmentsEvent(CombineFragmentsParameterData data)
		{
			var parameters = new Dictionary<string, object>()
			{
				{"pet_id", data.PetId },
				{"combine_amount", data.CombineAmount },
				{"fragment_amount", data.FragmentAmount },
				{"result_amount", data.ResultAmount },
			};
			_analytic.LogEvent(ANALYTIC_KEY.COMBINE_PET_FRAGMENT_EVENT, parameters);
		}

		public class CombineFragmentsParameterData
		{
			public string PetId { get; set; }
			public int CombineAmount { get; set; }
			public int FragmentAmount { get; set; }
			public int ResultAmount { get; set; }
			public CombineFragmentsParameterData(string petId, int combineAmount, int fragmentAmount, int resultAmount)
			{
				PetId = petId;
				CombineAmount = combineAmount;
				FragmentAmount = fragmentAmount;
				ResultAmount = resultAmount;
			}
		}

		public class SelectPetFragmentParameterData
		{
			public string PetId { get; set; }
			public bool IsCombineable { get; set; }
			public SelectPetFragmentParameterData(string petId, bool isCombineable) { PetId = petId; IsCombineable = isCombineable; }
		}
	}
}