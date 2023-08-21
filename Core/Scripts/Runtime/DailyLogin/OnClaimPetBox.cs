using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.DailyLogin
{
    public class OnClaimPetBox : IOnClaimDailyLogin
    {
        public object Meta { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void ClaimReward(DailyLoginRewardData dailyLoginRewardData)
        {
            var rewardPetBoxData = (RewardBase[])dailyLoginRewardData.RefObject;
            MainSceneController.Instance.Data.ProcessRewards(rewardPetBoxData);
        }

        public void ShowClaim(DailyLoginRewardData rewardData, Sprite rewardSprite, int DayReward, float duration, UnityAction<DailyLoginRewardData> onClaimReward, UnityAction onRewardPanelClose)
        {
            onClaimReward.Invoke(rewardData);
            var rewardPetBoxData = (RewardBase[]) rewardData.RefObject;
            LeanTween.delayedCall(duration, () =>
            {
                MainSceneController.Instance.Data.OnPetBoxObtained?.Invoke(rewardPetBoxData.ToList());
            });
        }
    }
}