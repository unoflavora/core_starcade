using Agate.Starcade.Scripts.Runtime.Data_Class;
using Agate.Starcade.Core.Runtime.Lobby;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Runtime.Enums;
using TMPro;
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Lobby;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class PosterObject : MonoBehaviour
    {
        [SerializeField] private float _posterMinimumHeight;
        [SerializeField] private Image _imageContainer;
        [SerializeField] private Button _posterActionButton;
        [SerializeField] private Button _claimButton;
        [SerializeField] private TMP_Text _claimTextButton;
        [SerializeField] private Color _claimTextColorActive;
        [SerializeField] private Color _claimTextColorDisable;
        [SerializeField] private GameObject _comingSoonMark;
        [SerializeField] private GameObject _claimedMark;
        [SerializeField] private NotificationBadge _notification;
        private RectTransform _rt;

        private IPosterAction _posterActionInterface;

        private string _rewardId;
        public string RewardId => _rewardId;
        
        private bool _isClaimableOnClickPoster;
        public bool IsClaimableOnClickPoster => _isClaimableOnClickPoster;

        private bool _IsDontHaveReward;
        private bool _isComingSoon;

        private AudioController _audio;

        private PosterManager _posterManager;

        public void SetupPoster(PosterData posterData,PosterManager posterManager ,UnityAction<string> posterAction ,UnityAction<string> claimAction, float height)
        {
            _posterMinimumHeight = height;
            _rewardId = posterData.PosterId;
            _posterManager = posterManager;
            _rt = this.gameObject.GetComponent<RectTransform>();
            
            if (posterData.IsComingSoon)
            {
                _isComingSoon = true;
                _posterActionButton.interactable = false;
                _claimButton.gameObject.SetActive(false);
                _claimTextButton.color = _claimTextColorDisable;
                _claimedMark.SetActive(false);
                _comingSoonMark.SetActive(true);
                _imageContainer.sprite = posterData.PosterSprite;
                _notification.DisableBadge();
                _rt.sizeDelta = GetPosterTargetSize(posterData.PosterSprite);
                return;
            }
            
            _imageContainer.sprite = posterData.PosterSprite;
            _rt.sizeDelta = GetPosterTargetSize(posterData.PosterSprite);
            
            _IsDontHaveReward = posterData.IsDontHaveReward;
            _isClaimableOnClickPoster = posterData.IsClaimableOnClickPoster;
            
            if (_IsDontHaveReward)
            {
                _claimButton.gameObject.SetActive(false);
                _claimedMark.gameObject.SetActive(false);
                return;
            }

            if (_isClaimableOnClickPoster)
            {
                _claimButton.gameObject.SetActive(true);
            }

            _posterActionInterface = GetActionInterface(posterData.PosterAction);

            _posterActionButton.onClick.AddListener(async () =>
            {
                MainSceneController.Instance.Audio.PlaySfx(BUTTON_GENERAL);
                try
                {
                    await _posterActionInterface.OnClickAction(posterData, () =>
                    {
                        _posterManager.OnOpen();
                    });
                    posterAction.Invoke(posterData.PosterId);
                    _notification.DisableBadge();
                }
                catch
                {
                    
                }
            });
            
            _claimButton.onClick.AddListener(() =>
            {
                claimAction.Invoke(posterData.PosterId);
                MainSceneController.Instance.Audio.PlaySfx(BUTTON_GENERAL);
            });
        }

        private void ClaimReward()
        {
            
        }

        public void SetClaimState(bool state)
        {
            if (_IsDontHaveReward && _isComingSoon)
            {
                return;
            }
            _claimButton.gameObject.SetActive(!state);
        }

        public void SetAvailableRewardState(bool state)
        {
            if (_IsDontHaveReward&& _isComingSoon)
            {
                return;
            }
            _claimButton.interactable = state;

            _claimTextButton.color = _claimButton.interactable ? _claimTextColorActive : _claimTextColorDisable;
        }
        public void SetViewState(bool viewed)
        {
            if (_IsDontHaveReward&& _isComingSoon)
            {
                return;
            }

            if (viewed)
            {
                _notification.DisableBadge();
            }
            else
            {
                _notification.EnableBadge();
            }
        }
        
        private IPosterAction GetActionInterface(PosterActionEnum posterActionEnum)
        {
            switch (posterActionEnum)
            {
                case PosterActionEnum.OpenUrl:
                    return new OpenUrlPosterAction();
                case PosterActionEnum.OpenSceneAdditive:
                    return new OpenAdditiveScenePosterAction();
                case PosterActionEnum.OpenRateUs:
                    return new OpenRateUsAction();
                default:
                    throw new ArgumentOutOfRangeException(nameof(posterActionEnum), posterActionEnum, null);
            }
        }
        
        private Vector2 GetPosterTargetSize(Vector2 size)
        {
            float gY = size.y;
            float gX = size.x;
            Debug.Log(gY);
            Debug.Log(gX);
            float pY = 562f; // still need find how to get this number
            float pX = (pY * gX / gY) - (gX * 140 / 600); //HARDCODE MAGIC NUMBER :(
            return new Vector2(pX, pY);
        }

        private Vector2 GetPosterTargetSize(Sprite sprite)
        {
            float gY = sprite.rect.size.y;
            float gX = sprite.rect.size.x;
            Debug.Log(gY);
            Debug.Log(gX);
            float pY = _posterMinimumHeight;
            float pX = (pY * gX / gY) - (gX * 140 / 600); //HARDCODE MAGIC NUMBER :(
            return new Vector2(pX, pY);
        }
    }
}