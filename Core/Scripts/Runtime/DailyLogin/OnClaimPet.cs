using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.DailyLogin
{
    public class OnClaimPet : IOnClaimDailyLogin
    {
        public object Meta { get; set; }

        void IOnClaimDailyLogin.ClaimReward(DailyLoginRewardData dailyLoginRewardData)
        {
            var pet = JsonConvert.DeserializeObject<PetInventoryData>(dailyLoginRewardData.RefObject.ToString());
            MainSceneController.Instance.Data.PetInventory.AddPet(pet);
        }

        void IOnClaimDailyLogin.ShowClaim(DailyLoginRewardData rewardData, Sprite rewardSprite, int DayReward, float duration, UnityAction<DailyLoginRewardData> onClaimReward, UnityAction onRewardPanelClose)
        {
            RewardBase rewardBase = new RewardBase()
            {
                Ref = rewardData.Ref,
                Amount = rewardData.Amount,
                RefObject = rewardData.RefObject,
                Type = rewardData.RewardType,
            };

            RewardBase[] rewardBases = new RewardBase[] { rewardBase };

            //Sprite[] sprite = { rewardSprite };

            //var refObject = (PetRef)rewardData.RefObject;

            //string [] refData = new string[] { refObject.PetName };

            onClaimReward.Invoke(rewardData);

            LeanTween.delayedCall(duration, () =>
            {
                MainSceneController.Instance.Info.ShowReward("Daily Login Reward Day " + DayReward, "Enjoy your reward!", rewardBases, new InfoAction("Close", () =>
                {
                    onRewardPanelClose.Invoke();
                }), null);

                //MainSceneController.Instance.Info.ShowReward("Daily Login Reward Day " + DayReward, "Enjoy your reward!", sprite, refData, new InfoAction("Close", () =>
                //{
                //    onRewardPanelClose.Invoke();
                //}), null);
            });
        }
    }
}