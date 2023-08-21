using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Enums;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Fragment.Dummy;
using Agate.Starcade.Core.Runtime.Pet.Fragment.Object;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet.PetFragmentAnalyticEventHandler;

namespace Agate.Starcade.Core.Runtime.Pet.Fragment.Controller
{
	public class PetFragmentController : MonoBehaviour
    {
        public static string PET_FRAGMENT_INTRO_SFX = "pet_fragment_intro";
        public static string PET_FRAGMENT_RESULT_SFX = "pet_fragment_result";

        [SerializeField] private Canvas _petFragmentCanvas;
        [SerializeField] private GameObject _petFragmentCardPrefab;
        [SerializeField] private Transform _petFragmentCardContainer;
        [SerializeField] private TMP_Dropdown _petFragmentDropdownSort;
        [SerializeField] private GameObject _petFragmentEmptyWarning;
        [SerializeField] private PetFragmentVFX _petFragmentVFX;
        [SerializeField] private PetFragmentEvent _petFragmentEvent;

        [Header("POPUP")]
        [SerializeField] private PetFragmentPopUpController _combinePopUp;
        [SerializeField] private PetFragmentPopUpController _obtainPopUp;
        [SerializeField] private PetFragmentPopUpController _confirmPopUp;


        [Header("DEBUG")]
        [SerializeField] private bool _useDummyData;
        [SerializeField] private PetFragmentsDummy _petFragmentsDummy;
        [SerializeField] private bool _useCheat;

        private List<PetFragmentInventory> _petFragmentDatas = new List<PetFragmentInventory>();
        private List<PetFragmentCard> _petFragmentCards = new List<PetFragmentCard>();

        private int _currentRequirmentFragment;
        private int _totalPet;

        private PetFragmentAnalyticEventHandler _analytic;

        private void Awake()
        {
            _petFragmentDropdownSort.onValueChanged.AddListener(Sort);

            
        }

        private async void Start()
        {
            await MainSceneController.Instance.Audio.LoadAudioData("pet_fragment_audio");
            _analytic = new PetFragmentAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _petFragmentCanvas.worldCamera = Camera.main;
            if (_useDummyData)
            {
                InitDummy();
            }
            else
            {
                Init();
            }
        }

        private async void Init()
        {
            _petFragmentDatas = MainSceneController.Instance.Data.PetFragment.Inventory;
            foreach (var data in _petFragmentDatas)
            {
                data.ObtainedDateTime = DateTime.Parse(data.ObtainedDate);
            }
            SetupCard();
            AssignData();
            InitEvent();
        }

        private void InitDummy()
        {
            _petFragmentDatas = _petFragmentsDummy.PetFragmentDatas;

            foreach (var data in _petFragmentDatas)
            {
                data.ObtainedDateTime = DateTime.Parse(data.ObtainedDate);
            }
            SetupCard();
            AssignData();
            InitEvent();
        }

        private void InitEvent()
        {
            _petFragmentEvent.OnStartVFX.AddListener(() =>
            {
                MainSceneController.Instance.Audio.PlaySfx(PET_FRAGMENT_INTRO_SFX);
            });

            _petFragmentEvent.OnResultVFX.AddListener(() =>
            {
                MainSceneController.Instance.Audio.PlaySfx(PET_FRAGMENT_RESULT_SFX);
            });
        }

        private void OnEnable()
        {
            if (_useCheat) ActivateCheat();
        }

        private void OnDisable()
        {
            if (_useCheat) DeactivateCheat();
        }

        private void SetupCard()
        {
            Debug.Log("CHILD COUNT CARD = " + _petFragmentCardContainer.childCount);
            Debug.Log("DATAS COUNT = " + _petFragmentDatas.Count);
            if (_petFragmentCardContainer.childCount <= _petFragmentDatas.Count)
            {
                int needed = _petFragmentDatas.Count - _petFragmentCardContainer.childCount;
                for (int i = 0; i < needed; i++)
                {
                    var temp = Instantiate(_petFragmentCardPrefab, _petFragmentCardContainer);
                    temp.SetActive(true);
                }
            }
            _petFragmentCards = _petFragmentCardContainer.GetComponentsInChildren<PetFragmentCard>(true).ToList();
            SortFragment(SortingTypeEnum.Default);
        }

        private void AssignData()
        {
            _petFragmentEmptyWarning.gameObject.SetActive(_petFragmentDatas.Count <= 0);
            _petFragmentDropdownSort.gameObject.SetActive(_petFragmentDatas.Count > 0);

            //ASSIGN DATA TO ITEM BUTTON
            for (int i = 0; i < _petFragmentCards.Count; i++)
            {
                if (_petFragmentDatas.Count <= i)
                {
                    _petFragmentCards[i].gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("INDEX = " + i);
                    _petFragmentCards[i].gameObject.SetActive(true);
                    _petFragmentCards[i].Setup(_petFragmentDatas[i], ShowPopUp);
                    _petFragmentCards[i].GetComponent<CanvasTransition>().TriggerTransition();
                }
            }
        }

        private async void Sort(int value)
        {
            foreach (var card in _petFragmentCards)
            {
                card.GetComponent<CanvasTransition>().TriggerFadeOut();
            }
            await Task.Delay(350);

            SortingTypeEnum sortingType = (SortingTypeEnum)value;
            SortFragment(sortingType);

            _analytic.SelectPetFragmentFilterEvent(sortingType.ToString());

            AssignData();
        }

        private void SortFragment(SortingTypeEnum sortingTypeEnum)
        {
            switch (sortingTypeEnum)
            {
                case SortingTypeEnum.Default:
                    _petFragmentDatas.Sort((x, y) => x.PetId.CompareTo(y.PetId));
                    break;
                case SortingTypeEnum.Quantity:
                    _petFragmentDatas.Sort((x, y) => x.Owned > y.Owned == true ? -1 : 1);
                    break;
                case SortingTypeEnum.Latest:
                    _petFragmentDatas.Sort((x, y) => DateTime.Compare(y.ObtainedDateTime, x.ObtainedDateTime));
                    break;
            }
        }

        enum PopUpFragmentState
        {
            Combine,
            Obtain,
            Confirm
        }

        private void ShowPopUp(PetFragmentInventory petFragmentInventory)
        {
            if(petFragmentInventory.Owned >= petFragmentInventory.RequirementAmount)
            {
                ShowPopUpCombine(petFragmentInventory);
            }
            else
            {
                ShowPopUpObtain(petFragmentInventory);
            }
        }

        private void ShowPopUpCombine(PetFragmentInventory petFragmentInventory)
        {
            _analytic.TrackClickPetFragmentItemEvent(new SelectPetFragmentParameterData(petFragmentInventory.PetId,true));

            _combinePopUp.gameObject.SetActive(true);
            _combinePopUp.Open();
            _combinePopUp.SetupTitle(petFragmentInventory.PetName + "'s Fragments", string.Empty);
            _combinePopUp.SetupImage(petFragmentInventory);
            _combinePopUp.SetupSlider(petFragmentInventory);
            _combinePopUp.SetupInputNumber(petFragmentInventory, petFragmentInventory.RequirementAmount);
            _combinePopUp.SetupAction(() =>
            {
                _combinePopUp.Close();
                ShowPopUpConfirmation(petFragmentInventory, _combinePopUp.PetFragmentInputNumber.Value, petFragmentInventory.RequirementAmount);
            }
            ,null);
        }

        private void ShowPopUpObtain(PetFragmentInventory petFragmentInventory)
        {
            _analytic.TrackClickPetFragmentItemEvent(new SelectPetFragmentParameterData(petFragmentInventory.PetId, false));

            _obtainPopUp.gameObject.SetActive(true);
            _obtainPopUp.Open();
            _obtainPopUp.SetupImage(petFragmentInventory);
            _obtainPopUp.SetupSlider(petFragmentInventory);
            _obtainPopUp.SetupAction(() =>
            {
                _analytic.TrackClickPetFragmentGotoStoreEvent(petFragmentInventory.PetId);
                MainSceneController.Instance.MainSceneManager.MoveToLobby(Starcade.Runtime.Enums.LobbyMenuEnum.StorePetbox);
                _obtainPopUp.Close();
            }
            ,() =>
            {
                _analytic.TrackClickPetFragmentGotoAdventureBoxEvent(petFragmentInventory.PetId);
                MainSceneController.Instance.MainSceneManager.MoveToPet(Starcade.Runtime.Enums.LobbyMenuEnum.AdventureBox, true);
                //_obtainPopUp.Close();
            });
        }

        private void ShowPopUpConfirmation(PetFragmentInventory petFragmentInventory, int totalPet, int totalCost)
        {
            _confirmPopUp.gameObject.SetActive(true);
            _confirmPopUp.Open();
            //_confirmPopUp.SetupTitle(null, "You are going to combine these fragments into pet.");
            _confirmPopUp.SetupImage(petFragmentInventory);
            _confirmPopUp.SetupMessage(petFragmentInventory.PetName + " x" + totalPet + "\n("+ (totalCost*totalPet) + " fragments consumed)");
            _confirmPopUp.SetupAction(() =>
            {
                CombineFragment(petFragmentInventory, totalCost, totalPet);
            }, () =>
            {
                _confirmPopUp.Close();
            });
        }

        private async void CombineFragment(PetFragmentInventory petFragmentInventory, int totalCostFragment, int totalAmount)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            CombinePetFragmentRequest request = new CombinePetFragmentRequest(petFragmentInventory.PetId, totalAmount);
            var combineRequest = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.CombineFragment(request));
            if (combineRequest.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(combineRequest.Error.Code);
                MainSceneController.Instance.Loading.DoneLoading();
                return;
            }

            //Debug.Log("combine = " + JsonConvert.SerializeObject(combineRequest));

            CombineFragmentsParameterData dataAnalytic = new CombineFragmentsParameterData(petFragmentInventory.PetId, totalAmount, totalCostFragment, combineRequest.Data.GrantedPets.Count);
            _analytic.TrackCombinePetFragmentsEvent(dataAnalytic);

            //UPDATE
            MainSceneController.Instance.Data.PetFragment.UpdatePetFragmentInvetory(combineRequest.Data.AffectedFragment);
            MainSceneController.Instance.Data.PetInventory.AddPet(combineRequest.Data.GrantedPets);

            string title = "Fragment Combination Successful";
            string caption = totalAmount == 1 ? "You got new pet!" : "You got " + totalAmount + " new pets!";
            Sprite icon = MainSceneController.Instance.AssetLibrary.GetPetObject(petFragmentInventory.PetId).PetSpriteAsset;
            string iconName = petFragmentInventory.PetName;

            _petFragmentVFX.StartVFX(MainSceneController.Instance.AssetLibrary.GetPetObject(petFragmentInventory.PetId).PetSpriteAsset, MainSceneController.Instance.AssetLibrary.GetPetObject(petFragmentInventory.PetId).FragmentSpriteAsset, () =>
            {
                //SHOW POPUP
                MainSceneController.Instance.Info.ShowReward(title, caption, new Sprite[] { icon }, new string[] { petFragmentInventory.PetName }, new InfoAction("Close", null), null);
            });

            
            //RE INIT
            if (_useDummyData) InitDummy();
            else Init();
      
            _confirmPopUp.Close();
          
            MainSceneController.Instance.Loading.DoneLoading();
        }

        private void ActivateCheat()
        {
            DebugLogConsole.AddCommand<string, int>("AddFragment", "Force add pet fragment", AddFragment);
        }

        private void DeactivateCheat()
        {
            DebugLogConsole.RemoveCommand("AddFragment");
        }

        private async void AddFragment(string petId, int amount)
        {
            await MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
            PetFragmentAddRequest request = new PetFragmentAddRequest(petId, amount);
            var addRequest = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.AddFragment(request));
            if (addRequest.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(addRequest.Error.Code);
                MainSceneController.Instance.Loading.DoneLoading();
                return;
            }
            var petsResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.GetPetsLibraryData());
            if (petsResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(petsResponse.Error.Code);
                MainSceneController.Instance.Loading.DoneLoading();
                return;
            }

            MainSceneController.Instance.Data.PetFragment.Inventory = petsResponse.Data.PetFragmentInventory;
            MainSceneController.Instance.Data.PetConfigs = petsResponse.Data.PetConfigs;

            if (_useDummyData)
            {
                InitDummy();
            }
            else
            {
                Init();
            }
            MainSceneController.Instance.Loading.DoneLoading();
        }
    }
}