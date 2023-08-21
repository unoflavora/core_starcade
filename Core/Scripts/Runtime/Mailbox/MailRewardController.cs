using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Core.Runtime;

namespace Agate.Starcade
{
    public class MailRewardController : MonoBehaviour
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
        [SerializeField] private CollectibleSlot _collectibleSlot;

        private ItemState _state = ItemState.General;

        private AssetLibrary _asset;

        private void Start()
        {
            _asset = MainSceneController.Instance.AssetLibrary;
        }

        public void SetContent(RewardBase content, bool isPanelVisible)
        {
            SetContent(content);
            SetPanelVisible(isPanelVisible);
        }

        public void SetContent(string itemId, string desc)
        {
            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(itemId);
            _desc.text = desc;
        }

        public void SetContent(RewardBase reward)
        {
            if (reward == null) return;
            SetImage(reward);
            SetTextReward(reward);
            SetUIState(reward);
        }

        private void SetImage(RewardBase reward)
        {
            switch (reward.Type)
            {
                case RewardEnum.GoldCoin:
                    _icon.sprite = _asset.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.StarCoin:
                    _icon.sprite = _asset.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.StarTicket:
                    _icon.sprite = _asset.GetSpriteAsset(reward.Type.ToString());
                    break;
                case RewardEnum.Avatar:
                    _icon.sprite = _asset.GetSpriteAsset(reward.Ref);
                    break;
                case RewardEnum.Frame:
                    break;
                case RewardEnum.Lootbox:
                    break;
                case RewardEnum.Collectible:
                    break;
                case RewardEnum.Pet:
                    break;
                case RewardEnum.PetFragment:
                    break;
                case RewardEnum.SpecialBox:
                    break;
                case RewardEnum.PetBox:
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
                    break;
                case RewardEnum.Pet:
                    break;
                case RewardEnum.PetFragment:
                    break;
                case RewardEnum.SpecialBox:
                    break;
                case RewardEnum.PetBox:
                    break;
                default:
                    break;
            }
        }

        private void SetUIState(RewardBase reward)
        {
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
                    break;
                case RewardEnum.PetFragment:
                    break;
                case RewardEnum.SpecialBox:
                    break;
                case RewardEnum.PetBox:
                    break;
                default:
                    break;
            }
        }

        public void SetPanelVisible(bool visible)
        {
            _squarePanel.enabled = visible;
        }

        private void SetupCollectible(RewardBase content)
        {
            if (_collectibleSlot == null) return;
            _collectibleSlot.gameObject.SetActive(true);
            _squarePanel.gameObject.SetActive(true);
            _desc.gameObject.SetActive(true);
            _collectibleSlot.SetupData(content.Ref, content.Amount);
            CollectiblesController.UpdateCollectibleAmountInModel(content.Ref, (int)content.Amount);
        }

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
