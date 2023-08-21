using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate
{
    public class ArcadeLevelProgressController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] TextMeshProUGUI _progressText;
        [SerializeField] Slider _progressBar;
        [SerializeField] Image _rewardIcon;
        [SerializeField] TextMeshProUGUI _rewardText;

        public void SetData(int level, int expProgress, int expTotal, RewardEnum type, long amount)
        {
            _level.text = $"LV. {level}";
            _progressText.text = $"XP {expProgress}/{expTotal}";
            //_progressBar.localScale = new Vector3(((float)expProgress / (float)expTotal), 1, 1);
            
            _rewardIcon.sprite = GetRewardSprite(type);
            _rewardText.text = CurrencyHandler.Convert(amount);

			LeanTween.cancel(this.gameObject);
            var targetExp = (float)expProgress / (float)expTotal;
            if(targetExp <= 0)
            {
               _progressBar.value = targetExp;
            }
            else
            {
			    LeanTween.value(this.gameObject, _progressBar.value, targetExp, 0.5f).setOnUpdate((float val) => _progressBar.value = val);
            }
		}

        public void ToggleVisible()
        {
            SetVisible(!gameObject.activeSelf);
        }

        public void SetVisible(bool visible)
        {
            Debug.Log("Activate");
            gameObject.SetActive(visible);
        }

        public Sprite GetRewardSprite(RewardEnum type)
        {
            Sprite sprite = null;
            if (type == RewardEnum.GoldCoin) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_goldcoin");
            else if (type == RewardEnum.StarCoin) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_starcoin");
            else if (type == RewardEnum.StarTicket) sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset("currency_default_starticket");
            return sprite;
        }
    }
}
