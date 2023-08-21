using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.Modules;
using Agate.Starcade.Core.Runtime.Pet.House.UI;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Backend;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.House
{
    public class PetHouseSceneController : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private PetHouseView _petListView;
        [SerializeField] private PetHouseStatusController _petHouseStatusController;
        
        private MainModel _mainModel;
        private PetHouseAnalyticEventHandler _analytic;

        // Start is called before the first frame update
        void Start()
        {
            _mainModel = MainSceneController.Instance.Data;
            _analytic = new PetHouseAnalyticEventHandler(MainSceneController.Instance.Analytic);
            
            _petListView.SetupAnalytic(_analytic);
            
            _petHouseStatusController.SetupAnalytic(_analytic);
            
            // InitDevelopmentMode();
            
            _petListView.Init();
            
            UpdatePetList();
            
            InitInteractions();
        }

        private void InitInteractions()
        {
            _petListView.OnPetItemClicked = OnPetItemClick;

            _petHouseStatusController.RegisterOnCloseClicked(() =>
            {
                UpdatePetList();
                _petHouseStatusController.gameObject.SetActive(false);
                _petListView.gameObject.SetActive(true);
            });
        }

        private void UpdatePetList()
        {
            var data = _mainModel.PetInventory;
            
            _petListView.DisplayPets(data);
        }

        private void OnPetItemClick(PetInventoryData data)
        {
            _analytic.TrackClickPetHouseItemEvent(data.Id);
            
            _petHouseStatusController.SetPetStatus(data);
            
            _petListView.gameObject.SetActive(false);
            
            _petHouseStatusController.gameObject.SetActive(true);
        }
        
        private async void InitDevelopmentMode()
        {
            CollectibleBackendController.InitDummyData();

            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();
            
            MainSceneController.Instance.Loading.DoneLoading();
        }
    }
}
