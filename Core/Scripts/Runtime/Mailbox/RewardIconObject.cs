using System;
using Agate.Starcade.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agate.Starcade.Core.Scripts.Runtime.UI.Reward;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Main;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Newtonsoft.Json;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Starcade.Core.Localizations;
using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Core.Runtime;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using static Agate.Starcade.LobbyAudio;

namespace Agate.Starcade
{
    public class RewardIconObject : MonoBehaviour
    {
        private enum ItemState
        {
            General,
            Collectible
        }

        [SerializeField] private Image _squarePanel;
        [SerializeField] private GameObject _generalItem;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private NotificationBadge _badge;
        [FormerlySerializedAs("_poolableSlot")][SerializeField] private CollectibleSlot _collectibleSlot;

        private ItemState _state = ItemState.General;

        private AssetLibrary _asset;

        private void Start()
        {
            _asset = MainSceneController.Instance.AssetLibrary;
        }

        private void OnEnable()
        {
            _badge.DisableBadge();
        }

        public void SetContent(RewardBase content, bool isPanelVisible)
        {
            SetContent(content);
            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
            //SetPanelVisible(isPanelVisible);
        }

        public void SetContent(string itemId, string desc)
        {
            Debug.Log("set info for = " + itemId);
            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(itemId);
            _desc.text = desc;
            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
        }

        public void SetContent(string itemId, string desc, bool useBadge = false)
        {
            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(itemId);
            _desc.text = desc;

            if (!useBadge)
            {
                _badge.DisableBadge();
            }
            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
        }

        //public void SetContent(RewardBase rewardObjectData)
        //{
        //    //_badge.DisableBadge();
        //    if (rewardObjectData == null) return;
        //    if (rewardObjectData.Type == RewardEnum.Collectible)
        //    {
        //        SetState(ItemState.Collectible);
        //        HandleCollectibleReward(rewardObjectData);
        //        return;
        //    }
        //    Sprite iconSprite = MainSceneController.Instance.AssetLibrary.GetRewardAsset(rewardObjectData);
        //    _icon.sprite = iconSprite;

        //    switch (rewardObjectData.Type)
        //    {
        //        case RewardEnum.GoldCoin:
        //            _badge.DisableBadge();
        //            _desc.text = CurrencyHandler.Convert(rewardObjectData.Amount);
        //            break;
        //        case RewardEnum.StarCoin:
        //            _badge.DisableBadge();
        //            _desc.text = CurrencyHandler.Convert(rewardObjectData.Amount);
        //            break;
        //        case RewardEnum.StarTicket:
        //            _badge.DisableBadge();
        //            _desc.text = CurrencyHandler.Convert(rewardObjectData.Amount);
        //            break;
        //        case RewardEnum.Avatar:
        //            _badge.DisableBadge();
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            break;
        //        case RewardEnum.Frame:
        //            _badge.DisableBadge();
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            break;
        //        case RewardEnum.Lootbox:
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            if (rewardObjectData.Amount > 1)
        //            {
        //                _badge.EnableBadgeWithDuplicated((int)rewardObjectData.Amount);
        //            }
        //            break;
        //        case RewardEnum.Collectible:
        //            _badge.DisableBadge();
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            break;
        //        case RewardEnum.Pet:
        //            _badge.DisableBadge();
        //            var pet = JsonConvert.DeserializeObject<PetInventoryData>(JsonConvert.SerializeObject(rewardObjectData.RefObject));

        //            if (PetInventoryExtensions.IsNewPet(pet.Id))
        //            {
        //                Debug.Log("new");
        //                _badge.EnableNewBadge();
        //            }
        //            else
        //            {
        //                Debug.Log("not new");
        //            }

        //            _desc.text = pet.Name;
        //            break;
        //        case RewardEnum.PetFragment:

        //            var petData = JsonConvert.DeserializeObject<PetFragmentRefObject>(rewardObjectData.RefObject.ToString());
        //            _desc.text = petData.PetName + "'s Fragment";
        //            if (rewardObjectData.Amount > 1)
        //            {
        //                _badge.EnableBadgeWithDuplicated((int)rewardObjectData.Amount);
        //            }
        //            break;
        //        case RewardEnum.SpecialBox:
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            if (rewardObjectData.Amount > 1)
        //            {
        //                _badge.EnableBadgeWithDuplicated((int)rewardObjectData.Amount);
        //            }
        //            break;
        //        case RewardEnum.PetBox:
        //            _desc.text = rewardObjectData.Ref.ToString();
        //            break;
        //    }



        //CHECK NEW HERE
        //}

        public void SetContent(Sprite iconSprite, long amount)
        {
            _icon.sprite = iconSprite;
            _desc.text = amount.ToString();
            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
        }

        public void SetContent(Sprite iconSprite, string amount)
        {
            PetAlbumData pet;
            if (MainSceneController.Instance.Data.PetAlbum != null)
            {
                pet = MainSceneController.Instance.Data.PetAlbum.Find(pet => pet.Id == amount);
                if (pet != null)
                {
                    _desc.text = pet.Name;
                }
                else
                {
                    amount = amount.Replace("_", " ");

                    if (amount.Length == 0)
                        amount = string.Empty;
                    else if (amount.Length == 1)
                        amount = char.ToUpper(amount[0]).ToString();
                    else
                        amount = char.ToUpper(amount[0]) + amount.Substring(1);

                    _desc.text = amount;
                }
            }
            else
            {
                _desc.text = amount;
            }
            _icon.sprite = iconSprite;
            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition();
        }

        public void SetContent(RewardBase reward, string sfx = null)
        {
            if (reward == null) return;
            SetImage(reward);
            SetTextReward(reward);
            SetUIState(reward);

            this.gameObject.GetComponent<CanvasTransition>().TriggerTransition(sfx);
        }

        private void SetImage(RewardBase reward)
        {
            
            switch (reward.Type)
            {
                case RewardEnum.GoldCoin:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.StarCoin:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.StarTicket:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.Avatar:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref);
                    break;
                case RewardEnum.Frame:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref);
                    break;
                case RewardEnum.Lootbox:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref);
                    break;
                case RewardEnum.Collectible:
                    break;
                case RewardEnum.Pet:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(reward.Ref)?.PetSpriteAsset;
                    break;
                case RewardEnum.PetFragment:
					_icon.sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(reward.Ref)?.FragmentSpriteAsset;
					break;
                case RewardEnum.SpecialBox:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref);
                    break;
                case RewardEnum.PetBox:
                    _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(reward.Ref);
                    break;
                default:
                    break;
            }
        }

        private void SetTextReward(RewardBase reward)
        {
            switch (reward.Type)
            {
                case RewardEnum.GoldCoin:
                    _desc.text = CurrencyHandler.Convert(reward.Amount);
                    break;
                case RewardEnum.StarCoin:
                    _desc.text = CurrencyHandler.Convert(reward.Amount);
                    break;
                case RewardEnum.StarTicket:
                    _desc.text = CurrencyHandler.Convert(reward.Amount);
                    break;
                case RewardEnum.Avatar:
                    _desc.text = "Avatar";
                    break;
                case RewardEnum.Frame:
                    _desc.text = "Frame";
                    break;
                case RewardEnum.Lootbox:
                    _desc.text = reward.Ref.Replace("_", " ");
                    break;
                case RewardEnum.Collectible:
                    break;
                case RewardEnum.Pet:
                    var pet = JsonConvert.DeserializeObject<PetInventoryData>(reward.RefObject.ToString());
                    _desc.text = pet.Name;
                    break;
                case RewardEnum.PetFragment:
                    var petFragment = JsonConvert.DeserializeObject<PetFragmentRefObject>(reward.RefObject.ToString());
                    _desc.text = petFragment != null ? petFragment.PetName + "'s Fragment" : "";
                    break;
                case RewardEnum.SpecialBox:
                    _desc.text = MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table, reward.Ref);
                    break;
                case RewardEnum.PetBox:
                    _desc.text = reward.Ref.Replace("_", " ");
                    break;
                default:
                    break;
            }
        }

        private void SetUIState(RewardBase reward)
        {
            _badge.DisableBadge();
            
            if(_collectibleSlot != null) _collectibleSlot.gameObject.SetActive(false);
            if(_generalItem != null) _generalItem.gameObject.SetActive(true);
            
            switch (reward.Type)
            {
                case RewardEnum.GoldCoin:
                    break;
                case RewardEnum.StarCoin:
                    break;
                case RewardEnum.StarTicket:
                    break;
                case RewardEnum.Avatar:
                    break;
                case RewardEnum.Frame:
                    break;
                case RewardEnum.Lootbox:
                    break;
                case RewardEnum.Collectible:
                    SetupCollectible(reward);
                    break;
                case RewardEnum.Pet:
                    if ((int)reward.Amount > 1)
                    {
                        _badge.EnableBadgeWithDuplicated((int)reward.Amount);
                    }
                    break;
                case RewardEnum.PetFragment:
                    if ((int)reward.Amount > 1)
                    {
                        _badge.EnableBadgeWithDuplicated((int)reward.Amount);
                    }
                    break;
                case RewardEnum.SpecialBox:
                    if((int)reward.Amount > 1)
                    {
                        _badge.EnableBadgeWithDuplicated((int)reward.Amount);
                    }
                    break;
                case RewardEnum.PetBox:
                    break;
                default:
                    break;
            }
        }

        private void SetupCollectible(RewardBase content)
        {
            if (_collectibleSlot == null) return;
            _collectibleSlot.gameObject.SetActive(true);
            _squarePanel.gameObject.SetActive(true);
            _generalItem.gameObject.SetActive(false);
            _desc.gameObject.SetActive(true);
             _collectibleSlot.SetupData(content.Ref, content.Amount);
        }

        //public void SetPanelVisible(bool visible)
        //{
        //    _squarePanel.enabled = visible;
        //}


        //private void HandleCollectibleReward(RewardBase content)
        //{
        //    ShowCollectiblePin();

        //    _collectibleSlot.SetupData (content.Ref, content.Amount);

        //    CollectiblesController.UpdateCollectibleAmountInModel(content.Ref, (int)content.Amount);
        //}

        //private void ShowCollectiblePin(bool enable = true)
        //{
        //    if(_collectibleSlot == null) return;

        //    _collectibleSlot.gameObject.SetActive(enable);

        //    _squarePanel.gameObject.SetActive(!enable);

        //    _desc.gameObject.SetActive(!enable);
        //}

        //private void SetState(ItemState state)
        //{
        //    SetItemVisible(_state, false);
        //    _state = state;
        //    SetItemVisible(_state, true);
        //}

        //private void SetItemVisible(ItemState state, bool visible)
        //{
        //    if (state == ItemState.General)
        //    {
        //        _generalItem.SetActive(visible);
        //        _desc.gameObject.SetActive(visible);
        //    }
        //    else if (state == ItemState.Collectible)
        //    {
        //        _collectibleSlot.gameObject.SetActive(visible);
        //    }
        //}

    }
} 
