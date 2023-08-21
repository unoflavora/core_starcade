using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox;
using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.SpecialBox;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agate.Starcade.Core.Runtime.Lobby.PetBox.Model;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Utilities;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox
{
    public class LootboxInfoUI : MonoBehaviour
    {
        [Header("Pins")]
        [SerializeField] List<GameObject> _slots;
        
        [Header("UIs")]
        [SerializeField] private Image _chestIcon;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _itemCountDescription;
        
        [Header("Interactables")]
        [SerializeField] private Button _button;

        public Action OnClose;

        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                gameObject.SetActive(false);
            });
        }

        #region LOOTBOX HANDLER

        public void InitData(LootboxData lootboxData)
        {
            var title = char.ToUpper(lootboxData.RarityConfig.RarityId[0]) +
                        lootboxData.RarityConfig.RarityId.Substring(1);
            
            var set = MainSceneController.Instance.Data.CollectiblesData
                .Find(set => set.CollectibleSetId == lootboxData.CollectibleSetId);
            
            SetText(title, set.CollectibleSetName, lootboxData.RarityConfig.Amount);
            SetPinConfiguration(lootboxData.RarityConfig.Configuration);
            SetRarity(lootboxData.LootboxItemId);
        }

        public void SetText(string title, string setTitle, int amount)
        {
            _title.SetText(title);
            
            _description.SetText("Collection set: " + setTitle);
            
            _itemCountDescription.SetText("Pins obtained: " + amount);
        }

        private void SetRarity(string rarityId)
        {
            _chestIcon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(rarityId);
        }

        #endregion

        #region PIN HANDLER
        public void SetPinConfiguration(List<RarityConfiguration> rarityConfiguration)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                
                if(i < rarityConfiguration.Count)
                {
                    slot.GetComponentInChildren<CollectibleSlot>().SetupData(new CollectibleItem()
                    {
                        Amount = 1,
                        CollectibleItemId = "",
                        CollectibleItemName = rarityConfiguration[i].ChanceRate + "%",
                        Rarity = rarityConfiguration[i].TargetRarity
                        
                    }, i, new GridUIOptions { ShowQuestionMark = true });
                    
                    slot.SetActive(true);
                    
                    continue;
                }
                
                slot.SetActive(false);
            }
        }
        #endregion

        #region SPECIAL BOX HANDLER

        public void ShowSpecialBoxInfo(AdventureBoxData adventureBox)
        {
            gameObject.SetActive(true);
            _title.text = adventureBox.Name;
            _chestIcon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(adventureBox.AdventureBoxId);
            _itemCountDescription.text = "Items Obtained: x" + adventureBox.Rewards.Length;

            for (int i = 0; i < _slots.Count; i++)
            {
                if (adventureBox.Rewards.Length <= i)
                {
                    _slots[i].gameObject.SetActive(false);
                }
                else
                {
                    _slots[i].gameObject.SetActive(true);
                    string reward = CurrencyHandler.Convert(adventureBox.Rewards[i].MinAmount) + " - " + CurrencyHandler.Convert(adventureBox.Rewards[i].MaxAmount);
                    _slots[i].GetComponent<RewardIconObject>().SetContent(adventureBox.Rewards[i].Type.ToString(), reward);
                }
            }
        }

        #endregion

        #region PET BOX HANDLER

        public void ShowPetBoxInfo(string petBoxName, PetBoxData petBox)
        {
            gameObject.SetActive(true);
            _title.text = petBoxName;
            _chestIcon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(petBox.PetBoxId);
            _description.text = "Box contains pet attributes";
            _itemCountDescription.text = "Items Obtained: x" + petBox.TotalItem;

            for (int i = 0; i < _slots.Count; i++)
            {
                if (petBox.Rewards.Length <= i)
                {
                    _slots[i].gameObject.SetActive(false);
                }
                else
                {
                    _slots[i].gameObject.SetActive(true);


                    string rewardName = string.Empty;
                    string spriteId = string.Empty;
                    switch (petBox.Rewards[i].Type)
                    {
                        case RewardEnum.GoldCoin:
                            spriteId = petBox.Rewards[i].Type.ToString();
                            rewardName = "Gold Coin";
                            break;
                        case RewardEnum.StarCoin:
                            spriteId = petBox.Rewards[i].Type.ToString();
                            rewardName = "Star Coin";
                            break;
                        case RewardEnum.StarTicket:
                            spriteId = petBox.Rewards[i].Type.ToString();
                            rewardName = "Star Ticket";
                            break;
                        case RewardEnum.Pet:
                            spriteId = "hidden_pet";
                            rewardName = "Pet";
                            break;
                        case RewardEnum.PetFragment:
                            spriteId = "hidden_pet_fragment";
                            rewardName = "Pet Fragment";
                            break;
                        default:
                            spriteId = "hidden_reward";
                            rewardName = "Hidden Reward";
                            break;
                    }

                    string reward = string.Empty;
                    if (petBox.Rewards[i].Amount > 1)
                    {
                        reward = rewardName + " (x" + petBox.Rewards[i].Amount + ")\n" + petBox.Rewards[i].Chance.ToString("0.00") + "%";
                    }
                    else
                    {
                        reward = rewardName + "\n" + petBox.Rewards[i].Chance.ToString("0.00") + "%";
                    }

                    _slots[i].GetComponent<RewardIconObject>().SetContent(spriteId, reward, false);
                }
            }
        }

        #endregion
    }
}
