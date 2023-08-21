using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Core.Runtime.Lobby.Store.Controller;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Core.Runtime.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Lobby.Store;
using Starcade.Core.Runtime.Lobby.Script.Lootbox.VFX;
using UnityEngine;
using UnityEngine.Serialization;
using static Agate.Starcade.Core.Runtime.Analytics.Handlers.StoreAnalyticEventHandler;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox
{
	public class LootboxStoreController : LobbyStore
    {
        [FormerlySerializedAs("_currentSeason")]
        [Header("Season")]
        [SerializeField] private Transform _premium;
        [FormerlySerializedAs("_pastSeason")] [SerializeField] private Transform regular;
        
        [Header("Reward Popup")]
        [SerializeField] private PoolableGridUI _rewardPopup;
        [SerializeField] private ConfirmationPopup _rewardConfirmation;
        
        [Header("Confirm Popup")]
        [SerializeField] private ConfirmationPopup _confirmPopup;
        [SerializeField] private StoreItemController _confirmItemPurchase;
        
        [Header("Lootbox Item Prefab")]
        [SerializeField] private GameObject _lootboxItemPrefab;
        private List<GameObject> _lootboxItems;
        
        [Header("UI")]
        [SerializeField] private GameObject _bgOverlay;
        [SerializeField] private LootboxInfoUI _lootboxInfo;

        // GAME DATA
        private List<ShopData> _shopDataList;
        private List<LootboxData> _lootboxDataList;
        private GameBackendController _gameBackend;

        // TODO, Update UI Balance with this
        public Action<PlayerBalance> OnPurchaseItem;

        public Action GoToCollectibles;
        private Action _goToLobby;
        private StoreAnalyticEventHandler _analytic;

        private void OnEnable()
        {
            _shopDataList = MainSceneController.Instance.Data.ShopData;
            
            _lootboxDataList = MainSceneController.Instance.Data.LootboxData;

            _gameBackend = MainSceneController.Instance.GameBackend;
        }
        
        public void RegisterOnInsufficientBalance(Action goToLobby)
        {
            _goToLobby = goToLobby;
        }

        public void RegisterAnalytics(StoreAnalyticEventHandler analytic)
        {
            _analytic = analytic;
        }
        public void Init()
        {
            _shopDataList = MainSceneController.Instance.Data.ShopData;

            _lootboxDataList = MainSceneController.Instance.Data.LootboxData;

            _gameBackend = MainSceneController.Instance.GameBackend;

            if (_lootboxItems == null) _lootboxItems = new List<GameObject>();
            
            else foreach(var item in _lootboxItems) item.SetActive(false);
            
            int pastSeasonCount = 0;

            List<ShopData> lootboxDatas = new List<ShopData>();


            foreach(ShopData shopData in _shopDataList)
            {
                if (shopData.itemConfig.StoreCategory != StoreCategoryTypeEnum.LootBox) continue;
                lootboxDatas.Add(shopData);
            }

            Debug.Log("TOTAL LOOTBOX = " + lootboxDatas.Count);

            foreach (var shopData in lootboxDatas)
            {
                var content = shopData.itemConfig;
                var lootboxData = _lootboxDataList.Find(data => data.LootboxItemId == shopData.itemConfig.Items[0].Ref);
                
                var parent = lootboxData.IsPremium ? _premium : regular;

                if (parent == regular) pastSeasonCount++;

                var itemContent = GetPooledObject(parent).GetComponent<StoreItemController>();


                itemContent.SetContent(shopData, lootboxData);
                itemContent.AddListener(() => OnStoreItemClick(shopData, lootboxData));
                itemContent.AddInfoListener(() => OnInfoClick(lootboxData));
            }

            regular.gameObject.SetActive(pastSeasonCount > 0);
        }

        private GameObject GetPooledObject(Transform parent)
        {
            for (int i = 0; i < _lootboxItems.Count; i++)
            {
                if (_lootboxItems[i].activeSelf == false)
                {
                    _lootboxItems[i].SetActive(true);
                    
                    _lootboxItems[i].transform.SetParent(parent);
                    
                    return _lootboxItems[i];
                }
            }
            
            var itemObject = Instantiate(_lootboxItemPrefab, parent);
            
            _lootboxItems.Add(itemObject);
            
            return itemObject;
        }
        
        private void OnStoreItemClick(ShopData shopData, LootboxData lootboxData)
        {
            _analytic.TrackClickStoreLootboxItemEvent(lootboxData.LootboxItemId);

            _confirmItemPurchase.SetContent(shopData, lootboxData);
            
            _confirmItemPurchase.AddInfoListener(() =>
            {
                CloseConfirmationPopup();
                
                OnInfoClick(lootboxData);
            });
            
            _bgOverlay.SetActive(true);
            
            _confirmPopup.gameObject.SetActive(true);
            
            _confirmPopup.AddListenerOnCancel(CloseConfirmationPopup);

            _confirmPopup.AddListenerOnConfirmation(async () => { await PurchaseLootboxItem(shopData, lootboxData); });
            
        }

        private async Task PurchaseLootboxItem(ShopData shopData, LootboxData lootboxData)
        {
            MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_GENERAL);

            CloseConfirmationPopup();

            Debug.Log("player balance: " + GetPlayerBalance().GoldCoin + " price: " +
                      shopData.itemConfig.Cost);

            await BuyShop(shopData, lootboxData.IsPremium, lootboxData.RarityType);
        }

        private void OnInfoClick(LootboxData lootboxData)
        {
            _analytic.TrackClickStoreLootboxItemInformationEvent(lootboxData.LootboxItemId);

            _bgOverlay.SetActive(true);
            
            _lootboxInfo.InitData(lootboxData);
            
            _lootboxInfo.gameObject.SetActive(true);
            
            _lootboxInfo.OnClose = () => _bgOverlay.SetActive(false);

        }

        private async Task BuyShop(ShopData shopData, bool isPremium, LootboxRarityEnum type)
        {
            _bgOverlay.SetActive(true);
            
            var result = await RequestHandler.Request(async () => await _gameBackend.ShopBuy(shopData.itemId));
            
            if (result.Error != null)
            {
                if (result.Error.Code == "10401")
                {
                    Debug.LogError("Insufficient Balance!");
                    MainSceneController.Instance.Info.Show(InfoType.InsufficientBalance,
                        new InfoAction("Go To Store", () => { _goToLobby?.Invoke(); }),
                        new InfoAction("Close", () => { }));
                    _bgOverlay.SetActive(false);
                    return;
                }
                
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                return;
            }

            List<PinParameterData> pinData = result.Data.LootboxGachaResult.Select(data => new PinParameterData(data.CollectibleItemId, data.Rarity)).ToList();
            BuyLootboxParameterData lootboxData = new BuyLootboxParameterData(result.Data.ItemId, result.Data.Pay.Type.ToString(), (decimal)result.Data.Pay.Amount, pinData);
            _analytic.TrackBuyStoreLootboxItemEvent(lootboxData);

            UpdatePlayerBalance(shopData.itemConfig.Cost, shopData.itemConfig.CostCurrency);

            var gachaResult = result.Data.LootboxGachaResult;
            
            // Update amount in database
            foreach(var item in gachaResult) CollectiblesController.UpdateCollectibleAmountInModel(item.CollectibleItemId);;

            DisplayPurchasedLootboxItems(type, isPremium, result.Data.LootboxGachaResult);
        }

        private void DisplayPurchasedLootboxItems(LootboxRarityEnum type, bool isPremium, List<LootboxGachaResult> gachaResult)
        {
            var items = GetRewardItems(gachaResult);
            
            MainSceneController.Instance.Data.OnLootboxObtained?.Invoke(items, isPremium, type, () => DisplayRewardPopup(items, type));
        }

        private void DisplayRewardPopup(List<CollectibleItem> items, LootboxRarityEnum type)
        {            
            _bgOverlay.SetActive(false);

            List<RewardBase> rewards = new List<RewardBase>();
            foreach (var item in items)
            {
                var reward = new RewardBase()
                {
                    Amount = 1,
                    Chance = 0,
                    Ref = item.CollectibleItemId,
                    RefObject = null,
                    Type = RewardEnum.Collectible
                };
                rewards.Add(reward);
            }

            var rewardsArray = rewards.ToArray();

            var title = $"Your {Enum.GetName(typeof(LootboxRarityEnum), type)} Lootbox";
            
            MainSceneController.Instance.Info.ShowReward(title, "You've got new Collectibles!",rewardsArray, 
            new InfoAction("Collectibles", () => GoToCollectibleScene()), new InfoAction("Close", () => {}));
            
        }

        private List<CollectibleItem> GetRewardItems(List<LootboxGachaResult> gachaResult)
        {
            var collectibleResult = new List<CollectibleItem>();

            foreach (var item in gachaResult)
            {
                var collectible = CollectibleItem.FindCollectibleItemById(item.CollectibleItemId);

                collectibleResult.Add(new CollectibleItem()
                {
                    CollectibleItemId = collectible.CollectibleItemId,
                    CollectibleItemName = collectible.CollectibleItemName,
                    Rarity = collectible.Rarity,
                    Amount = 1,
                });
            }

            return collectibleResult;
        }

        private void CloseConfirmationPopup()
        {

            _bgOverlay.SetActive(false);

            _confirmPopup.gameObject.SetActive(false);
        }
        
        private void EnableRewardPopup(bool enable)
        {
            _bgOverlay.SetActive(enable);

            _rewardConfirmation.gameObject.SetActive(enable);
            
            _rewardConfirmation.ResetScroll();
        }
        
        private void UpdatePlayerBalance(double deductedBalance, CurrencyTypeEnum currencyType)
        {
            switch (currencyType)
            {
                case CurrencyTypeEnum.GoldCoin:
                    MainSceneController.Instance.Data.UserBalance.GoldCoin -= deductedBalance;
                    break;
                case CurrencyTypeEnum.StarCoin:
                    MainSceneController.Instance.Data.UserBalance.StarCoin -= deductedBalance;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    MainSceneController.Instance.Data.UserBalance.StarTicket -= deductedBalance;
                    break;
            }

            MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged?.Invoke(MainSceneController.Instance.Data.UserBalance);
        }

        private PlayerBalance GetPlayerBalance()
        {
            return MainSceneController.Instance.Data.UserBalance;
        }
        
        private void GoToCollectibleScene()
        {
            MainSceneController.Instance.MainSceneManager.MoveToUserProfile(LobbyMenuEnum.Collection);
        }
    }
}
