using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using Agate.Starcade.Scripts.Runtime.Info;
using UnityEngine.Events;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Data;
using System.Collections.Generic;

namespace Agate.Starcade.Core.Runtime.DailyLogin
{
    public class OnClaimCurrency : IOnClaimDailyLogin
    {
        public object Meta { get; set; }

        void IOnClaimDailyLogin.ClaimReward(DailyLoginRewardData dailyLoginRewardData)
        {
            RewardBase reward = new RewardBase()
            {
                Type = dailyLoginRewardData.RewardType,
                Amount = dailyLoginRewardData.Amount,
                Ref = dailyLoginRewardData.Ref,
                RefObject = dailyLoginRewardData.RefObject,
            };
            MainSceneController.Instance.Data.ProcessReward(reward);
        }

        void IOnClaimDailyLogin.ShowClaim(DailyLoginRewardData dailyLoginRewardData, Sprite rewardSprite, int DayReward, float duration, UnityAction<DailyLoginRewardData> onClaimReward, UnityAction onRewardPanelClose)
        {
            RewardBase rewardBase = new RewardBase()
            {
                Ref = dailyLoginRewardData.Ref,
                Amount = dailyLoginRewardData.Amount,
                RefObject = dailyLoginRewardData.RefObject,
                Type = dailyLoginRewardData.RewardType,
            };

            RewardBase[] rewardBases = new RewardBase[] { rewardBase };

            //Sprite[] sprite = { rewardSprite };
            //string[] refData = new string[] { CurrencyHandler.Convert(dailyLoginRewardData.Amount) };

            onClaimReward.Invoke(dailyLoginRewardData);

            LeanTween.delayedCall(duration, () =>
            {
                MainSceneController.Instance.Info.ShowReward("Daily Login Reward Day " + DayReward, "Enjoy your reward!", rewardBases,new InfoAction("Close", () =>
                {
                    onRewardPanelClose.Invoke();
                }), null);
            });
        }
    }
}