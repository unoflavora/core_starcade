using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet
{
	public class PetAlbumAnalyticEventHandler : BaseAnalyticEventHandler
	{
		public static class ANALYTIC_KEY
		{
			public const string CLICK_PET_ALBUM_ITEM_EVENT = "click_pet_album_item";
			public const string CLICK_PET_ALBUM_ITEM_OBTAIN_BUTTON_EVENT = "click_pet_album_item_obtain_button";
		}

		public PetAlbumAnalyticEventHandler(IAnalyticController analytic) : base(analytic)
		{

		}

		public void TrackClickPetAlbumItemEvent(string id)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_ALBUM_ITEM_EVENT, "pet_id", id);
		}
		public void TrackClickPetAlbumItemObtainButtonEvent(string id)
		{
			_analytic.LogEvent(ANALYTIC_KEY.CLICK_PET_ALBUM_ITEM_OBTAIN_BUTTON_EVENT, "pet_id", id);
		}
	}
}