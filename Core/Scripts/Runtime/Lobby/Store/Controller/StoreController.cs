using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Game;
using Agate.Starcade.Core.Runtime.Lobby.Lootbox;
using Agate.Starcade.Core.Runtime.Lobby.Store;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.Store.Controller
{
    public class StoreController : LobbyStore
    {
        [SerializeField] private Transform _content;
        [SerializeField] private BestDealController _bestDeal;
        [SerializeField] private Toggle _storeToggle;

        [Header("Best Deal SO")]
        [SerializeField] private BestDealSO _generalBestDealSO;
        [SerializeField] private BestDealSO _lootboxBestDealSO;

        [Header("TEMP DATA, REMOVE LATER")]
        [SerializeField] private GeneralStoreSO _generalData;
        //[SerializeField] private LootboxStoreController _lootboxStoreController;

        private StoreAnalyticEventHandler _storeAnalyticEvent;
        private UserAnalyticEventHandler _userAnalyticEvent { get; set; }
        private GameBackendController _gameBackend;

        public async void Init()//Action onGoToCollectibles, Action<PlayerBalance> OnPurchaseItem)
        {
            _userAnalyticEvent = new UserAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _gameBackend = MainSceneController.Instance.GameBackend;

            await GetShopData(() =>
            {
                //gameObject.SetActive(true);
                GetComponent<FadeTween>().FadeIn();
            });

            SetContent(_generalData, async (id) =>
            {
                // Implement buy api
                await BuyShop(id);
                _storeAnalyticEvent.TrackBuyStoreCoinsItemEvent(null);
            });

            //int bestDealId = 6;
            AddBestDeaListener(async (id) =>
            {
                MainSceneController.Instance.Audio.PlaySfx(MainSceneController.AUDIO_KEY.BUTTON_SHOP_BUY);
                bool isSucces = await BuyShop(id);
                if (isSucces) ResetBestDealCooldown();
                _storeAnalyticEvent.TrackBuyStoreCoinsItemEvent(null);
            });
        }

        public void RegisterAnayltic(StoreAnalyticEventHandler analytic)
        {
            _storeAnalyticEvent = analytic;
        }

        #region CONTENT HANDLER
        public void SetContent(GeneralStoreSO content, UnityAction<string> onClick)
        {
            foreach (var item in content.items)
            {
                GameObject itemObject = Instantiate(content.itemPrefab, _content);

                item.Background = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(item.BackgroundAssetId);
                item.Item = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(item.ItemAssetId);

                StoreItemController itemController = itemObject.GetComponent<StoreItemController>();
                itemController.SetContent(item);
                itemController.AddListener(() => onClick(item.ItemId));
            }
        }

        public void SetBestDealContent(BestDealSO content)
        {
            content.Background = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(content.BackgroundAssetId);
            content.Label = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(content.LabelAssetId);
            _bestDeal.SetContent(content);
        }

        public void AddBestDeaListener(UnityAction<string> onClick)
        {
            _bestDeal.AddBestDeaListener(onClick);
        }

        public void SetBestDealCooldown(float cooldown)
        {
            _bestDeal.SetCooldown(cooldown);
        }

        public void ResetBestDealCooldown()
        {
            _bestDeal.ResetCooldown();
        }

        private void OnDestroy()
        {
            _storeToggle?.onValueChanged?.RemoveAllListeners();
        }

        #endregion

        private async Task GetShopData(UnityAction OnComplete)
        {
            var result = await RequestHandler.Request(async () => await _gameBackend.GetShopItems());
            if (result.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                return;
            }

            MainSceneController.Instance.Loading.DoneLoading();

            SetGeneralBestDealData(result.Data);
            OnComplete();

        }

        private void SetGeneralBestDealData(ShopData[] data)
        {
            ShopData bestDeal = data[0];
            _generalBestDealSO.ItemId = bestDeal.itemId;
            _generalBestDealSO.Interval = bestDeal.itemConfig.Interval;
            SetBestDealContent(_generalBestDealSO);
            if (bestDeal.lastPurchase != null)
            {
                Debug.Log(bestDeal.lastPurchase);
                SetBestDealCooldown(bestDeal.remainingSecond);
            }
            else
            {
                SetBestDealCooldown(0f);
            }
        }

        private async Task<bool> BuyShop(string id)
        {
            var result = await RequestHandler.Request(async () => await _gameBackend.ShopBuy(id));
            if (result.Error != null)
            {
                Debug.Log("Failed Buy");
                MainSceneController.Instance.Info.ShowSomethingWrong(result.Error.Code);
                return false;
            }
            else
            {
                _storeAnalyticEvent.TrackBuyStoreCoinsItemEvent(null);
                Debug.Log("Buy Success");
                UpdatePlayerBalance(result.Data.GrantedItems);
                return true;
            }
        }

        private void UpdatePlayerBalance(RewardBase[] data)
        {
            var goldCoin = MainSceneController.Instance.Data.UserBalance.GoldCoin;
            var starCoin = MainSceneController.Instance.Data.UserBalance.StarCoin;
            var starTicket = MainSceneController.Instance.Data.UserBalance.StarTicket;
            foreach (RewardBase item in data)
            {
                if (item.Type == RewardEnum.GoldCoin)
                {
                    goldCoin += item.Amount;
                    _userAnalyticEvent.TrackUserBalance(CurrencyTypeEnum.GoldCoin, item.Amount);
                }
                else if (item.Type == RewardEnum.StarCoin)
                {
                    starCoin += item.Amount;
                    _userAnalyticEvent.TrackUserBalance(CurrencyTypeEnum.StarCoin, item.Amount);
                }
                else if (item.Type == RewardEnum.StarTicket)
                {
                    starTicket += item.Amount;
                    _userAnalyticEvent.TrackUserBalance(CurrencyTypeEnum.StarTicket, item.Amount);
                }
            }
            PlayerBalance balance = new PlayerBalance()
            {
                GoldCoin = goldCoin,
                StarCoin = starCoin,
                StarTicket = starTicket
            };

            MainSceneController.Instance.Data.UserBalance = balance;


            MainSceneController.Instance.Data.PlayerBalanceActions.OnBalanceChanged(MainSceneController.Instance.Data.UserBalance);

            // _onBuyComplete.Invoke(MainSceneController.Instance.PlayerData.balance);
        }
    }
}
