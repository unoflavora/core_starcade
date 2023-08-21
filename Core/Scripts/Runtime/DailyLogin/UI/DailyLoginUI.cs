using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Data.DailyLogin;
using Agate.Starcade.Scripts.Runtime.DailyLogin.Data;
using Agate.Starcade.Scripts.Runtime.DailyLogin.SO;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.UI
{
    public class DailyLoginUI : MonoBehaviour
    {
        public const string WRONG_CLAIM_MESSAGE = "Please claim today's reward first. You can claim this reward on the designated day.";
        public const string ALREADY_CLAIM_MESSAGE = "You’ve already collected today’s reward, feel free to close this page.";

        [SerializeField] private List<RewardItemUi> _rewardItemUis;
        [SerializeField] private SessionTimer _sessionTimer;
        [SerializeField] private Button _closeButton;

        [SerializeField] private GameObject _blocker;

        [Header("Character Message")]
        [SerializeField] private GameObject _dialogBox;
        [SerializeField] private TMP_Text _dialogText;
        [SerializeField] private int _dialogDelay;
        private bool _dialogActive;

        public UnityEvent<DailyLoginRewardData, Sprite,int> OnClaim = new UnityEvent<DailyLoginRewardData,Sprite,int>();

        private bool _doneClaiming;
        private DailyLoginData _currentDailyLoginData;
        private DailyLoginSO _dailyLoginSO;


        public void ShowUI(bool show)
        {
            gameObject.SetActive(show);
        }

        public void ShowAfter()
        {
            gameObject.SetActive(true);
            SetUI(_currentDailyLoginData, _dailyLoginSO);
        }

        public void SetUI(DailyLoginData dailyLoginData, DailyLoginSO dailyLoginSo)
        {
            var dailyLoginRewardDatas = dailyLoginData.RewardList;
            var sessionDuration = 0;
            if (dailyLoginData.ResetDay != null && dailyLoginData.LastClaimDate != null)
                sessionDuration = dailyLoginData.ResetDay.Value.Second - dailyLoginData.LastClaimDate.Value.Second;

            Debug.Log("set timer");
            _sessionTimer.InitTimer(dailyLoginData.ResetDay, true, sessionDuration);

            Debug.Log("full daily login reward " + JsonConvert.SerializeObject(dailyLoginData));

            if (!_doneClaiming)
            {
                _closeButton.gameObject.SetActive(false);
                for (var i = 0; i < _rewardItemUis.Count; i++)
                {
                    _rewardItemUis[i].SetupReward(dailyLoginRewardDatas[i], dailyLoginSo);
                    _rewardItemUis[i].SetupButtonEvent(dailyLoginRewardDatas[i], ClaimAction);
                }

                Debug.Log("daily login reward " + JsonConvert.SerializeObject(dailyLoginRewardDatas));

                _currentDailyLoginData = dailyLoginData;
                _dailyLoginSO = dailyLoginSo;
            }
            else
            {
                _closeButton.gameObject.SetActive(true);
            }
        }

        public void SetUIAfterClaim(DailyLoginData dailyLoginData, DailyLoginSO dailyLoginSo)
        {
            var dailyLoginRewardDatas = dailyLoginData.RewardList;
            var sessionDuration = 0;
            if (dailyLoginData.ResetDay != null && dailyLoginData.LastClaimDate != null)
                sessionDuration = dailyLoginData.ResetDay.Value.Second - dailyLoginData.LastClaimDate.Value.Second;
            _sessionTimer.InitTimer(dailyLoginData.ResetDay, true, sessionDuration);
            _closeButton.gameObject.SetActive(true);

            for (var i = 0; i < _rewardItemUis.Count; i++)
            {
                if (dailyLoginRewardDatas[i].IsClaim == RewardStatusEnum.Granted)
                {
                    dailyLoginRewardDatas[i].IsClaim = RewardStatusEnum.Claimed;
                }

                _rewardItemUis[i].SetupReward(dailyLoginRewardDatas[i], dailyLoginSo);
                _rewardItemUis[i].SetupButtonEvent(dailyLoginRewardDatas[i], ClaimAction);
            }
            _doneClaiming = true;
        }

        private void ClaimAction(bool currentReward, Sprite rewardSprite)
        {
            if (currentReward)
            {
                ClaimReward();
                OnClaim.Invoke(_currentDailyLoginData.Reward, rewardSprite,_currentDailyLoginData.DayCount);
            }
            else
            {
                ShowCharacterMessage(currentReward);
            }
        }

        private async void ClaimReward()
        {
            _doneClaiming = true;
            ShowUI(false);
            //_blocker.SetActive(true);
            await Task.Delay(4000);
            //ShowUI(true);
            //_blocker.SetActive(false);
            //SetUI(_currentDailyLoginData, _dailyLoginSO);
        }

        private async void ShowCharacterMessage(bool currentReward)
        {
            if (_dialogActive) return;
            _dialogActive = true;
            _dialogBox.SetActive(true);

            if (!_doneClaiming)
            {
                if (!currentReward)
                {
                    _dialogText.text = WRONG_CLAIM_MESSAGE;
                }
            }
            else
            {
                _dialogText.text = ALREADY_CLAIM_MESSAGE;
            }
            await Task.Delay(_dialogDelay * 1000);
            
            _dialogBox.gameObject.GetComponent<CanvasTransition>().TriggerFadeOut();
            await Task.Delay(750);
            _dialogActive = false;
            _dialogBox.SetActive(false);
        }

        public void InitEvent(UnityAction onClose)
        {
            _closeButton.onClick.AddListener(onClose);
        }
    }
}