using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Reward_Panel;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using Agate.Starcade.Scripts.Runtime.Data_Class.Reward;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Reward
{
    public class CollectibleReward : MonoBehaviour
    {
        [SerializeField] private CollectibleRewardUIPanel _rewardUIPanel;
       
        private CollectibleRewardData _userRewardData;
        
        //STATE
        private int _currentPinCount;
        private int _currentMaxCount;
        private Sprite _rewardSprite;
        private bool _isClaimed = false;
        
        private void Start()
        {
            _rewardUIPanel.OnClaimReward = () => CollectibleActionController.OnClaimReward();
        }

        public void SetReward(CollectibleRewardData userRewardData)
        {
            _userRewardData = userRewardData;
            
            _isClaimed = _userRewardData is { Status: UserRewardEnum.Claimed };

            EnableClaimButton(_userRewardData is { Status: UserRewardEnum.Granted});

            var rewardImage = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(userRewardData.@ref);
            
            _rewardUIPanel.SetRewardImage(rewardImage);
        }

        public void SetPinCount(int currentPinCount, int currentMaxCount)
        {
            _currentPinCount = currentPinCount;
            _currentMaxCount = currentMaxCount;
            _rewardUIPanel.SetCollectibleCount(_currentPinCount, _currentMaxCount);

            if (_currentPinCount == _currentMaxCount && _isClaimed == false) EnableClaimButton(true);
            
            else  EnableClaimButton(false);
        }

        private void EnableClaimButton(bool enable)
        {
            _rewardUIPanel.EnableClaimButton(enable);
        }
    }
}
