using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using Agate.Starcade.Scripts.Runtime.Collectibles.Backend;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Model;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Core.Runtime.Analytics.Controllers;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Album;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Data_Class.Reward;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using ConfirmationPopup = Agate.Starcade.Core.Runtime.Lobby.ConfirmationPopup;
using Agate.Starcade.Runtime.Main;
using IngameDebugConsole;
using Agate.Starcade.Core.Runtime;
using Agate.Starcade.Scripts.Runtime.Game;
using Starcade.Core.Runtime.ScriptableObject;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core
{
    using UserCollectibles = Dictionary<string, List<CollectibleItem>>;
    
    public class CollectiblesController : MonoBehaviour
    {
        [SerializeField] private CollectibleSet _collectibleSet;
        [SerializeField] private CollectibleAlbum _collectibleAlbum;
        [SerializeField] private CollectiblePopupController _popupController;
        [SerializeField] private CollectibleShareController _shareController;
        [SerializeField] private List<ComingSoonCollectibleSo> _comingSoonCollectibles;
        
        private CollectibleBackendController _backendController;
        private AssetLibrary _assetLibrary;
        
        //DATA
        private MainModel _mainModel;
        private CollectibleAnalyticEventHandler _analytic;

        //STATE
        private CollectibleSetData _currentOpenedSet;

        private void Start()
        {
            _assetLibrary = MainSceneController.Instance.AssetLibrary;
            _analytic = new CollectibleAnalyticEventHandler(MainSceneController.Instance.Analytic);
            
            _collectibleSet.gameObject.SetActive(false);
            
            _backendController = new CollectibleBackendController();
            
            _mainModel = MainSceneController.Instance.Data;
            
            
            if (_mainModel.CollectiblesData == null) InitDevelopmentMode();
            
            InitComingSoonCollectibles();
            
            InitListeners();
            
            _collectibleAlbum.SetAlbumData(_mainModel.CollectiblesData);

            DebugLogConsole.AddCommand("add_collectibles", "Add Collectibles to Set", async () =>
            {
                var data = await _backendController.AddCollectibles();
                
                _mainModel.CollectiblesData = data;
                
                _collectibleAlbum.SetAlbumData(_mainModel.CollectiblesData);
            });
        }

        private void InitComingSoonCollectibles()
        {
            _comingSoonCollectibles.ForEach(comingSoonSet =>
            {
                if (comingSoonSet == null) return;
                
                var isAdded = _mainModel.CollectiblesData.Find(i => i.CollectibleSetId == comingSoonSet.SetId);
                
                if (isAdded != null) return;
                
                var currentCollectibleCount = _mainModel.CollectiblesData.Count + 1;
                
                comingSoonSet.SetId = "coming_soon_collectible_" + currentCollectibleCount;
                
                var collectibleSetComingSoon = new CollectibleSetData()
                {
                    CollectibleSetId = comingSoonSet.SetId,
                    CollectibleSetName = "Collectible Set " + currentCollectibleCount,
                    CollectibleItems = null,
                    IsComingSoon = true
                };

                _mainModel.CollectiblesData.Add(collectibleSetComingSoon);
            });
        }

        private async void InitDevelopmentMode()
        {
            
            await _assetLibrary.LoadAllAssets();

            CollectibleBackendController.InitDummyData();
                
            MainSceneController.Instance.Loading.DoneLoading();
                
            _collectibleAlbum.SetAlbumData(_mainModel.CollectiblesData);
           
        }

        private void InitListeners()
        {
            _collectibleAlbum.OnAlbumClicked = OpenCollectionSet;
            
            _collectibleSet.OnBackClicked = OpenCollectionAlbum;

            _popupController.GoToLootboxStore = (itemId) =>
            {
                _analytic.TrackCollectibleAlbumItemBuyButtonEvent(_currentOpenedSet.CollectibleSetId, itemId);

                UserProfileController.BackToLobby(LobbyMenuEnum.StoreLootbox);
            };
            
            CollectibleActionController.OnConvertPinConfirmed = CombineCollectible;
            CollectibleActionController.OnClaimReward = ClaimReward;
            CollectibleActionController.OnPinClicked = ShowPopup;
            CollectibleActionController.OnConvertPinPopupClicked = ShowConvertPopup;
            CollectibleActionController.OnShareClicked = Share;
            CollectibleActionController.OnSendPinClicked = ShowFriendPopup;
        }

        private void ShowFriendPopup(CollectibleItem item)
        {
            _popupController.DisplayFriendListPopup((friendToSend) => SendPin(friendToSend, item));
        }

        protected virtual void SendPin(FriendProfile profile, CollectibleItem itemToSend)
        {
            var data = new CurrentFriendInteractionData(profile.FriendCode, profile.Username);
            
            FriendsController.SendGiftToFriend(data, itemToSend, onBuySuccess: () =>
            {
                // refresh displayed collectible set
                
                _currentOpenedSet = _mainModel.CollectiblesData.Find(set => set.CollectibleSetId == _currentOpenedSet.CollectibleSetId);
                
                OpenCollectionSet(_currentOpenedSet);
            });
        }

        private void OpenCollectionSet (CollectibleSetData setToOpen)
        {
            _analytic.TrackClickCollectibleAlbumEvent(setToOpen.CollectibleSetId);

            if (setToOpen.IsComingSoon) return;
            
            _collectibleAlbum.gameObject.SetActive(false);
            
            _currentOpenedSet = setToOpen;
            
            _collectibleSet.ResetSlot();

            _collectibleSet.SetCollectionSetReward(setToOpen.Reward);
            
            _collectibleSet.SetCollectionSetTitle(setToOpen.CollectibleSetName);
            
            foreach (CollectibleItem collectibleItem in _currentOpenedSet.CollectibleItems)
            {
                _collectibleSet.AddItemToSlot(collectibleItem);
            }
            
            _collectibleSet.gameObject.SetActive(true);
        }

        private void OpenCollectionAlbum()
        {
            _analytic.TrackClickCollectibleAlbumBackButtonEvent(_currentOpenedSet.CollectibleSetId);


            // Refresh collectible data
            _collectibleAlbum.SetAlbumData(_mainModel.CollectiblesData);
            
            _collectibleSet.gameObject.SetActive(false);
            
            _collectibleAlbum.gameObject.SetActive(true);
        }

        private async void CombineCollectible(List<CollectibleItem> collectibles)
        {
           var result = await _backendController.CombineCollectible(collectibles, _currentOpenedSet.CollectibleSetId);
           
           if (result == null) return;
           
           _popupController.PlayConvertPinAnimation(() => OnSuccessCombiningPin(collectibles, result));
        }

        private void OnSuccessCombiningPin(List<CollectibleItem> collectibles, CollectibleItem result)
        {
            _analytic.TrackConvertCollectibleAlbumItemsSuccessEvent(new CollectibleAnalyticEventHandler.ConvertCollectiblePinParameterData()
            {
                SetId = _currentOpenedSet.CollectibleSetId,
                Pins = collectibles.Select(d=>new CollectibleAnalyticEventHandler.PinParameterData()
                {
                    Id = d.CollectibleItemId,
                    Rarity = d.Rarity
                }).ToList(),
                ResultPinId = result.CollectibleItemId,
                ResultPinRarity = result.Rarity,
			});

            foreach (var combinedItem in collectibles)
            {
                _collectibleSet.RemoveItem(combinedItem);
            }

            _collectibleSet.AddItemToSlot(result);

            _popupController.CloseAllPopup();

            _popupController.OpenItemPopup(result, true, false);
        }

        private async void ClaimReward()
        {
            _analytic.TrackClaimCollectibleAlbumRewardEvent(_currentOpenedSet.CollectibleSetId);

            CollectibleUserRewardData claimed = await _backendController.ClaimReward(_currentOpenedSet.CollectibleSetId);
            Debug.Log(JsonConvert.SerializeObject(claimed));
            if (claimed != null)
            {
                MainSceneController.Instance.Info.Show(
                    "You did it!",
                    "You have successfully claimed your reward!",
                    _assetLibrary.GetSpriteAsset(claimed.Ref),
                    new InfoAction
                        ("Close", () =>
                        {
                            UpdateReward(claimed);
                        }),
                    null);
                
                _currentOpenedSet.Reward.Status = UserRewardEnum.Claimed;
                
                _collectibleSet.SetCollectionSetReward(_currentOpenedSet.Reward);
            }
        }

        private void UpdateReward(CollectibleUserRewardData reward)
        {
            switch (reward.TypeReward)
            {
                case RewardEnum.Avatar:
                    _mainModel.AccessoryData.AvatarLibrary.UnlockedItems.Add(reward.Ref);
                    break;
                case RewardEnum.Frame:
                    _mainModel.AccessoryData.FrameLibrary.UnlockedItems.Add(reward.Ref);
                    break;
                case RewardEnum.Collectible:
                    UpdateCollectibleAmountInModel(reward.Ref);
                    _collectibleSet.UpdateInventory(); // refresh set UI
                    break;
            }
        }
        
        private void ShowPopup(Transform item)
        {
            var itemData = item.GetComponent<CollectibleSlot>().ItemData;
            
            _analytic.TrackClickCollectibleAlbumItemEvent(_currentOpenedSet.CollectibleSetId, itemData.GetItemId());

            _popupController.OpenItemPopup(itemData);
        }
        
        private void ShowConvertPopup(List<CollectibleItem> items)
        {
            _popupController.CloseAllPopup();
            
            _popupController.OpenConvertPinPopup(items);
            
            _analytic.TrackClickConvertCollectibleAlbumItemsEvent(_currentOpenedSet.CollectibleSetId);
        }
        
        private async void Share(List<CollectibleItem> data) 
        {
            _analytic.TrackClickShareCollectibleAlbumEvent(_currentOpenedSet.CollectibleSetId);
            
            _collectibleSet.gameObject.SetActive(false);
            
            _popupController.EnablePopupBackground(true);

            await _shareController.Capture(data, _currentOpenedSet.CollectibleSetName, () => _popupController.EnablePopupBackground(false));
            
            _collectibleSet.gameObject.SetActive(true);
            
        }

        private Dictionary<string, string> GetCollectibleItemData(string itemId)
        {
            return new Dictionary<string, string>()
            {
                {"set_id", _currentOpenedSet.CollectibleSetId},
                {"item_id", itemId},
            };
        }

        private void OnDestroy()
        {
            DebugLogConsole.RemoveCommand("add_collectibles");
        }


        /// <summary>
        /// Updates collectible amount in model, use this method when you want to update collectible amount in model
        /// </summary>
        /// <param name="collectibleId"> id of the collectible item</param>
        public static void UpdateCollectibleAmountInModel(string collectibleId, int amount = 1)
        {
            foreach (CollectibleSetData set in MainSceneController.Instance.Data.CollectiblesData)
            {
                CollectibleItem foundItem = set.CollectibleItems.Find(item => item.GetItemId() == collectibleId);

                if (foundItem != null)
                {
                    var rewardItem = foundItem;
                    rewardItem.Amount += amount;
                    break;
                }
            }
        }

    }
}
