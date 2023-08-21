using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.UI.Reward_Panel
{
    public class CollectibleRewardUIPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _collectibleCount;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Image _rewardImage;

        public Action OnClaimReward;

        private void Start()
        {
            _claimButton.onClick.AddListener(() => OnClaimReward());
        }

        public void SetCollectibleCount(int currentCount, int maxCount)
        {
            _collectibleCount.text = currentCount + " / " + maxCount;
        }

        public void EnableClaimButton(bool enabled)
        {
            _claimButton.interactable = enabled;
        }
        
        public void SetRewardImage(Sprite sprite)
        {
            _rewardImage.sprite = sprite;
        }
        
    }
}
