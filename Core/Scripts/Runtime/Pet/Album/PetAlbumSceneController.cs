using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Album.UI;
using Agate.Starcade.Core.Runtime.Pet.Core;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Album
{
    public class PetAlbumSceneController : MonoBehaviour
    {
        [SerializeField] private PetAlbumView _petAlbumView;
        [SerializeField] private PetAlbumDescription _petAlbumDescription;
        private PetAlbumAnalyticEventHandler _analytic;

        private void Start()
        {
            _analytic = new PetAlbumAnalyticEventHandler(MainSceneController.Instance.Analytic);

            _petAlbumView.gameObject.SetActive(true);

            var uniqueData = MainSceneController.Instance.Data.PetAlbum;

            var userPetCount = MainSceneController.Instance.Data.PetAlbum.FindAll(album => album.HasOwned).Count;

            _petAlbumView.DisplayPets(uniqueData, userPetCount, OnPetClick);
        }
        
        private void OnPetClick(PetAlbumData pet)
        {
            _analytic.TrackClickPetAlbumItemEvent(pet.Id);
                
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET023);

            _petAlbumView.GetComponent<CanvasGroup>().alpha = 0;
                
            _petAlbumDescription.DisplayPet(pet, () => _petAlbumView.GetComponent<CanvasGroup>().alpha = 1, () =>
            {
                _analytic.TrackClickPetAlbumItemObtainButtonEvent(pet.Id);

                PetSceneController.BackToLobby(LobbyMenuEnum.StorePetbox);
            });

        }
    }
}
