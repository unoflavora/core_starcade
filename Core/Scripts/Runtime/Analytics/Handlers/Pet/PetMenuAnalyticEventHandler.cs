using Agate.Starcade.Core.Runtime.Analytics.Controllers;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class PetMenuAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string CLICK_MY_PETS_TAB_EVENT = "click_my_pets_tab";
			public const string CLICK_PET_FRAGMETS_TAB_EVENT = "click_pet_fragments_tab";
			public const string CLICK_PET_HOUSE_TAB_EVENT = "click_pet_house_tab";
			public const string CLICK_PET_ALBUM_TAB_EVENT = "click_pet_album_tab";
			public const string CLICK_PET_ADVENTURE_BOX_TAB_EVENT = "click_adventure_box_tab";

			public const string CLICK_CLOSE_PET_MENU_EVENT = "click_close_pet_menu";
		}

		public PetMenuAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void TrackClickMyPetsTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_MY_PETS_TAB_EVENT);
		}

		public void TrackClickPetFragmentTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_FRAGMETS_TAB_EVENT);
		}

		public void TrackClickPetHouseTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_HOUSE_TAB_EVENT);
		}

		public void TrackClickPetAlbumTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_ALBUM_TAB_EVENT);
		}

		public void TrackClickAdventureBoxTabEvent()
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_ADVENTURE_BOX_TAB_EVENT);
		}

		public void TrackClickClosePetMenuEvent(string location)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_CLOSE_PET_MENU_EVENT, "location", location);
		}
	}
}