using Agate.Starcade.Core.Runtime.DailyLogin;
using Agate.Starcade;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Scripts.Runtime.Data_Class.Lootbox;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.DailyLogin
{
    public class OnClaimLootbox : IOnClaimDailyLogin
    {
        public object Meta { get; set; }

        private List<CollectibleItem> _lootboxCollectibleItem = new List<CollectibleItem>();

        public string CollectibleScenePath;

        private UnityEvent _onRewardPanelClose = new UnityEvent();

        void IOnClaimDailyLogin.ClaimReward(DailyLoginRewardData dailyLoginRewardData)
        {
            var gachaResult = ConvertRefObject(dailyLoginRewardData.RefObject);
            var items = GetRewardItems(gachaResult.LootboxGachaResults);
            foreach (var item in items)
            {
                CollectiblesController.UpdateCollectibleAmountInModel(item.CollectibleItemId);
            }
        }

        void IOnClaimDailyLogin.ShowClaim(DailyLoginRewardData rewardData, Sprite rewardSprite, int DayReward, float duration, UnityAction<DailyLoginRewardData> onClaimReward, UnityAction onRewardPanelClose)
        {
            _onRewardPanelClose.AddListener(onRewardPanelClose);
            var gachaResult = ConvertRefObject(rewardData.RefObject);
            var items = GetRewardItems(gachaResult.LootboxGachaResults);

            LootboxRarityEnum rarityEnum;

            switch (gachaResult.LootboxData.RarityType)
            {
                case "gold":
                    rarityEnum = LootboxRarityEnum.Gold;
                    break;
                case "silver":
                    rarityEnum = LootboxRarityEnum.Silver;
                    break;
                case "bronze":
                    rarityEnum = LootboxRarityEnum.Bronze;
                    break;
                default:
                    rarityEnum = LootboxRarityEnum.Bronze;
                    break;
            }

            onClaimReward.Invoke(rewardData);
            LeanTween.delayedCall(duration, () =>
            {
                MainSceneController.Instance.Data.OnLootboxObtained?.Invoke(items, false, rarityEnum, () => 
                {
                    DisplayPopUp(rewardData);
                });
            });
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

            _lootboxCollectibleItem = collectibleResult;

            return collectibleResult;
        }

        private void DisplayPopUp(DailyLoginRewardData rewardData)
        {
            List<Sprite> collectibleIcon = new List<Sprite>();
            List<string> collectibleName = new List<string>();

            foreach (var item in _lootboxCollectibleItem)
            {
                collectibleIcon.Add(MainSceneController.Instance.AssetLibrary.GetSpriteAsset(item.CollectibleItemId));
                collectibleName.Add(item.GetDisplayName());
            }

            MainSceneController.Instance.Info.ShowReward("Daily Login Reward Day " + rewardData.Day, "Enjoy your reward!", collectibleIcon.ToArray(), collectibleName.ToArray(), new InfoAction("Close", () =>
            {
                _onRewardPanelClose.Invoke();
                _onRewardPanelClose.RemoveAllListeners();
                //MainSceneController.Instance.Scene.MoveToUserProfile(LobbyMenuEnum.Collection);
            }), null);
        }

        private LootboxClaimRef ConvertRefObject(object refObject)
        {
            string gachaString = JsonConvert.SerializeObject(refObject);
            return JsonConvert.DeserializeObject<LootboxClaimRef>(gachaString);
        }


    }
}