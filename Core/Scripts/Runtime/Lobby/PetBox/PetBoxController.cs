using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Lobby.PetBox.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Core.Runtime.Lobby;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Lobby.Store;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using static Agate.Starcade.Core.Runtime.Analytics.Handlers.StoreAnalyticEventHandler;
using static Agate.Starcade.Runtime.Lobby.StoreManager;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox
{
    public class PetBoxController : LobbyStore
    {
        public static string PET_BOX_REWARD_POPUP = "lootbox_reward_popup";

        [SerializeField] private Image _mainPetImage;
        [SerializeField] private Image _mainEggImage;
        [SerializeField] private TMP_Text _petBoxTitle;
        [SerializeField] private TMP_Text _petBoxSliderTitle;
        [SerializeField] private SliderTransition _petBoxProgressSlider;
        [SerializeField] private TMP_Text _petBoxProgressText;
        [SerializeField] private Button _petBoxBuyButton;
        [SerializeField] private TMP_Text _petBoxBuyButtonText;
        [SerializeField] private TMP_Text _petBoxBuyButtonCurrencyText;
        [SerializeField] private Button _infoPetBoxButton;

        [Header("PopUp")]
        [SerializeField] private CanvasTransition _curtain;
        [SerializeField] private LootboxInfoUI _petBoxInfoPopUp;
        [SerializeField] private PetBoxConfirmationPopUp _confirmationPopup;
        [SerializeField] private GameObject _petBoxBlocker;

        [Header("Gacha Scene")]
        [SerializeField] private AssetReference _gachaScene;

        [Header("Dummy Data")]
        [SerializeField] private bool _useDummy;
        [SerializeField] private string _dummyPetBoxName;
        [SerializeField] private ShopData _dummyUserPetBoxData;
        [SerializeField] private ShopDataConfig _dummyUserPetBoxConfig;
        [SerializeField] private PetBoxData _dummyPetBox;

        //private string _petBoxName;
        private ShopData _userShopPetBoxData;
        private ShopDataConfig _userShopPetBoxConfig;
        private PetBoxData _petBox;

        private UnityAction<StoreType> _changeStoreTab;

        private StoreAnalyticEventHandler _analytic;

        public async void Init(UnityAction<StoreType> OnChangeTab)
        {
            if (_useDummy)
            {
                await MainSceneController.Instance.AssetLibrary.LoadAllAssets();
                _userShopPetBoxData = _dummyUserPetBoxData;
                _userShopPetBoxConfig = _dummyUserPetBoxConfig;
                _petBox = _dummyPetBox;
            }
            else
            {
                _userShopPetBoxData = MainSceneController.Instance.Data.ShopData.Find( data => data.itemConfig.StoreCategory == Starcade.Runtime.Enums.StoreCategoryTypeEnum.PetBox);
                _userShopPetBoxConfig = _userShopPetBoxData.itemConfig;
                _petBox = JsonConvert.DeserializeObject<PetBoxData>(JsonConvert.SerializeObject(_userShopPetBoxConfig.Items[0].RefObject));
            }

            MainSceneController.Instance.Data.OnPetBoxObtained = StartGachaScene;
            await MainSceneController.Instance.Audio.LoadAudioData("gacha_audio");

            _changeStoreTab = OnChangeTab;

            SetupUI();
        }

        public void RegisterAnalytic(StoreAnalyticEventHandler analytic)
        {
            _analytic = analytic;
        }

        private void OnEnable()
        {
            _infoPetBoxButton.onClick.AddListener(() => _petBoxInfoPopUp.ShowPetBoxInfo(_userShopPetBoxData.itemName,_petBox));
            _petBoxBuyButton.onClick.AddListener(ShowConfirmation);
        }

        private void OnDisable()
        {
            _infoPetBoxButton.onClick.RemoveAllListeners();
            _petBoxBuyButton.onClick.RemoveAllListeners();
        }

        private void SetupUI()
        {
            _petBoxTitle.text = _userShopPetBoxData.itemName;
            _petBoxSliderTitle.text = "Guaranteed Pet After " + _petBox.PityAmount + " Purchases!";

            _petBoxProgressSlider.ValueTransition(0, _petBox.PityAmount, _petBox.RollCount);
            _petBoxProgressText.text = _petBox.RollCount + "/" + _petBox.PityAmount;

            int currencyTypeInt = (int)_userShopPetBoxConfig.CostCurrency;
            _petBoxBuyButtonCurrencyText.text = "<sprite=" + currencyTypeInt.ToString() + ">";
            _petBoxBuyButtonText.text = CurrencyHandler.Convert(_userShopPetBoxConfig.Cost);
        }

        private void ShowConfirmation()
        {
			_analytic.TrackClickBuyPetboxItemEvent();

            Sprite sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(_petBox.PetBoxId); //MainSceneController.Instance.AssetLibrary.GetPetObject(_petBox.PetBoxId).Sprite;
            string title = _userShopPetBoxData.itemName;
            int currencyTypeInt = (int)_userShopPetBoxConfig.CostCurrency;
            _confirmationPopup.ShowConfirmation(sprite, title, (float)_userShopPetBoxConfig.Cost, "<sprite=" + currencyTypeInt.ToString() + ">", BuyPetBox);
        }

        private async void BuyPetBox()
        {
            if (_useDummy)
            {
                _petBox.RollCount++;
                var resultGacha = Gacha(_petBox.RollCount >= _petBox.PityAmount);
                ShowResult(resultGacha);
                if (_petBox.RollCount >= _petBox.PityAmount) _petBox.RollCount = 0;
                SetupUI();
            }
            else
            {
                var buyPetBoxResponse = await RequestHandler.Request(async () => await MainSceneController.Instance.GameBackend.ShopBuy(_userShopPetBoxData.itemId));

                if (buyPetBoxResponse.Error != null)
                {
                    if (buyPetBoxResponse.Error.Code == "10401")
                    {
                        Debug.LogError("Insufficient Balance!");
                        MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,
                            new InfoAction("Go To Store",() => { _changeStoreTab.Invoke(StoreType.General); }),
                            new InfoAction("Close", () => { }));
                        return;
                    }

                    MainSceneController.Instance.Info.ShowSomethingWrong(buyPetBoxResponse.Error.Code);
                    return;
                }

                List<PetboxResultParameterData> resultAnalytic = buyPetBoxResponse.Data.GrantedItems.Select(data => new PetboxResultParameterData()
                {
                    Id = data.Ref,
                    Type = data.Type.ToString(),
                    Amount = (int)data.Amount
                }).ToList();

                BuyPetboxParameterData buyPetboxParameterData =
                    new BuyPetboxParameterData
                    (buyPetBoxResponse.Data.ItemId,
                    _userShopPetBoxConfig.CostCurrency.ToString(),
                    _userShopPetBoxConfig.Cost,
                    _petBox.RollCount,
                    resultAnalytic);



                MainSceneController.Instance.Data.PlayerBalanceActions.ReduceBalance(_userShopPetBoxConfig.CostCurrency, _userShopPetBoxConfig.Cost);

                _analytic.TrackBuyStorePetboxItemEvent(buyPetboxParameterData);

                _petBoxBlocker?.SetActive(true);

                StartGachaScene(buyPetBoxResponse.Data.GrantedItems.ToList());

                _petBox.RollCount++;
                
                MainSceneController.Instance.Data.ProcessRewards(buyPetBoxResponse.Data.GrantedItems);
                if (_petBox.RollCount >= _petBox.PityAmount) _petBox.RollCount = 0;
                SetupUI();
            }
        }


        private List<RewardBase> Gacha(bool Guaranteed = false)
        {
            List<RewardBase> listReward = new List<RewardBase>();

            List<int> weights = _petBox.Rewards.Select((reward) => (int)reward.Chance).ToList();

            for (int i = 0; i < (Guaranteed ? 4 : 5); i++)
            {
                int randomWeight = Random.Range(0, weights.Sum());
                for (int y = 0; y < weights.Count; ++y)
                {
                    randomWeight -= weights[y];
                    if (randomWeight < 0)
                    {
                        listReward.Add(_petBox.Rewards[y]);
                        break;
                    }
                }
            }
            if (Guaranteed) listReward.Add(_petBox.Rewards[_petBox.PityIndex]);
            Debug.Log(JsonConvert.SerializeObject(listReward));
            return listReward;
        }

        private void ShowResult(List<RewardBase> rewards)
        {
            _petBoxBlocker?.SetActive(false);
            MainSceneController.Instance.Audio.PlaySfx("lootbox_box_result");
            MainSceneController.Instance.Info.ShowReward("Your Pet Box", "Please enjoy your reward", rewards.ToArray(), new InfoAction("Close", () =>
            {
                _curtain.TriggerFadeOut();
            }), new InfoAction("Roll Again", () => 
            {
                _curtain.TriggerFadeOut();
                ShowConfirmation();
            }),null, PET_BOX_REWARD_POPUP);
        }

        private async void StartGachaScene(List<RewardBase> rewards)
        {
            _curtain.TriggerTransition();

            await Task.Delay(250);

            UnityAction afterGacha = new UnityAction( () => 
            {
                ShowResult(rewards);
            });

            InitAdditiveBaseData data = new InitAdditiveBaseData();
            data.Data = afterGacha;
            LoadSceneHelper.PushData(data);

            LoadSceneHelper.LoadSceneAdditive(_gachaScene,data);

        }
    }
}