using Agate.Starcade.Core.Scripts.Runtime.UI.Reward;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Starcade.Core.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.VFX
{
    public class RewardVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _rewardParticleSplash;
        [SerializeField] private ParticleSystem _rewardIconSpin;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private NumberCounter _amount;
        [SerializeField] private RewardVFXSO _rewardVFXSO;
        [SerializeField] private TMP_Text _titleReward;

        [SerializeField] private Material _defaultRewardParticleSplash;

        private ParticleSystemRenderer _rewardParticleSplashRenderer;
        private ParticleSystemRenderer _rewardIconSpinRenderer;


        private void Awake()
        {
			_rewardParticleSplashRenderer = _rewardParticleSplash.GetComponent<ParticleSystemRenderer>();
			_rewardIconSpinRenderer = _rewardIconSpin.GetComponent<ParticleSystemRenderer>();

		}

        public void Init(float amount, Starcade.Runtime.Enums.RewardEnum rewardEnum, string titleText = "")
        {
			_titleReward.text = titleText;
            SetMaterial(_rewardVFXSO.GetVFXSprite(rewardEnum));
            SetAmountText(CurrencyHandler.Convert((double)amount));
        }


        public void InitDailyRewardVFX(float amount, RewardEnum rewardType, string titleText = "")
        {
            _titleReward.text = titleText;
            SetMaterial(_rewardVFXSO.GetVFXSprite(rewardType));
            SetAmountText(amount);
        }

        public void InitDailyRewardNonVFX(DailyLoginRewardData dailyLoginRewardData)
        {
            //Sprite sprite = AssetRewardHelper.GetRewardAsset(dailyLoginRewardData.Ref);
            Sprite sprite = null;
            switch (dailyLoginRewardData.RewardType)
            {
                case RewardEnum.GoldCoin:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.StarCoin:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.StarTicket:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.Avatar:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.Frame:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.Lootbox:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.Collectible:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.Pet:
                    sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(dailyLoginRewardData.Ref).PetSpriteAsset;
                    break;
                case RewardEnum.PetFragment:
					sprite = MainSceneController.Instance.AssetLibrary.GetPetObject(dailyLoginRewardData.Ref).FragmentSpriteAsset;
					break;
                case RewardEnum.SpecialBox:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
                case RewardEnum.PetBox:
                    sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset((string)dailyLoginRewardData.Ref);
                    break;
            }

            SetNonVFX(sprite);
            SetAmountText(dailyLoginRewardData.Amount);
        }

        private void SetMaterial((Material,Material) material)
        {

			_rewardParticleSplash.gameObject.SetActive(true);
			_rewardIconSpin.gameObject.SetActive(true);
            _rewardImage.gameObject.SetActive(false);

            _rewardParticleSplash.GetComponent<ParticleSystemRenderer>().material = material.Item1 != null ? material.Item1 : null;
            _rewardIconSpin.GetComponent<ParticleSystemRenderer>().material = material.Item2 != null ? material.Item2 : null;
        }

        private void SetNonVFX(Sprite spriteReward)
        {
            _rewardParticleSplashRenderer.gameObject.SetActive(true);
            _rewardIconSpinRenderer.gameObject.SetActive(false);
            _rewardImage.gameObject.SetActive(true);

            _rewardParticleSplashRenderer.material = _defaultRewardParticleSplash;
            _rewardImage.sprite = spriteReward;
        }

        private void SetAmountText(float amount)
        {
            if (amount <= 1)
            {
                _amount.Text.text = string.Empty;
            }
            else
            {
                _amount.Text.text = amount.ToString();
                _amount.Balance = amount;
            }
        }

        private void SetAmountText(string amount)
        {
            _amount.Text.text = amount;
 
        }

    }
}