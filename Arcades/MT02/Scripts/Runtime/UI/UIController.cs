using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Data.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility;
using Agate.Starcade.Scripts.Runtime.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.UI
{
    public enum GridOverlayText { InsufficientMatch, StartToPlay }
    
    public class UIController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button _mainSpinButton;
        [SerializeField] private List<Button> spinButtons;
        [SerializeField] private SessionTimer timer;
        [SerializeField] private GameObject goldMode;
        [SerializeField] private GameObject starMode;
        [SerializeField] private Button changeMode;
        [SerializeField] private Button SettingMenu;
        [SerializeField] private TextMeshProUGUI spinCount;
        [SerializeField] private GameObject _components;
        [SerializeField] private GameObject _gridUI;
        private Image _mainSpinImage;

        [Header("UI of User Profile")]
        [SerializeField] private Image _photoImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Button _profileButton;
        [SerializeField] private ArcadeLevelProgressController _levelProgressController;
        [SerializeField] private ArcadeLevelUpController _levelUpController;
        [SerializeField] private ArcadeMilestoneController _milestoneController;

        [Header("Store Buttons")]
        [SerializeField] private Button _gotoStoreGoldButton;
        [SerializeField] private Button _gotoStoreStarButton;

        [Header("UI VFXs")]
        [SerializeField] private GameObject _blinking;
        [SerializeField] private Image overlayBackground;
        [SerializeField] private Animator _clickStartToPlayVFX;
        [SerializeField] private Animator insufficientMatch;
        [SerializeField] private DaytimeTransitionHelper _daytimeTransitionHelper;
        [SerializeField] private Material _shineButtonMaterial;
        
        #region State
        private bool _insufficientMatchPopupDisplayed;
        private bool _startToPlayDisplayed;
        #endregion

        #region Events
        private UnityEvent _onClickSpin;
        public UnityEvent OnChangeModeClicked;
        #endregion

        private void Awake()
        {
            _onClickSpin = new UnityEvent();
            OnChangeModeClicked = new UnityEvent();
            
            foreach(Button spin in spinButtons)
            {
                if (spin.onClick != null)
                {
                    spin.onClick.AddListener(delegate { _onClickSpin.Invoke(); });
                }
            }

            EnableSpinCount(false);
        }


        public void SetProfilePhoto(Sprite photo)
		{
            if (photo == null) return;
			_photoImage.sprite = photo;
		}

        public void SetFramePhoto(Sprite photo)
        {
            if (photo == null)
            {
                _frameImage.gameObject.SetActive(false);
                return;
            }
            _frameImage.sprite = photo;
            _frameImage.gameObject.SetActive(true);
        }

        public void SetAllButtonInteractableState(bool isActive)
        {
            SetMainSpinButtonActive(isActive);
            spinButtons.ForEach(spin => spin.interactable = isActive);
            changeMode.interactable = isActive;
            SettingMenu.interactable = isActive;
        }

        public void SetMainSpinButtonActive(bool isActive)
        {
            _mainSpinButton.interactable = isActive;
            
            if(_mainSpinImage == null) _mainSpinImage = _mainSpinButton.GetComponent<Image>();
            _mainSpinImage.material = _mainSpinButton.interactable ? _shineButtonMaterial : null;
        }

        public void SetupTimer(string end, bool isStarted, int sessionDuration)
        {
            var endDate = DateTime.Parse(end).ToUniversalTime();
            
            timer.InitTimer(endDate, isStarted, sessionDuration);
        }
        
        
        public void SetTimeOfDay(DaytimeEnums daytimeEnums)
        {
            _daytimeTransitionHelper.SetTimeOfDay(daytimeEnums);
        }


        public void SetupMode(GameModeEnum gameMode)
        {
            goldMode.SetActive(gameMode == GameModeEnum.Gold);
            starMode.SetActive(gameMode == GameModeEnum.Star);

            changeMode.onClick.AddListener(delegate { OnChangeModeClicked.Invoke(); });
        }

        public void RegisterOnClickSpin(UnityAction callback)
        {
            _onClickSpin.AddListener(callback);
        }

        public void RegisterOnClickSetting(UnityAction callback)
        {
            SettingMenu.onClick.AddListener(callback);
        }

        public void RegisterOnFinishTimer(UnityAction callback)
        {
            timer.OnSessionTimeout.AddListener(callback);
        }

        public void EnableSpinCount(bool enable)
        {
            spinCount.gameObject.SetActive(enable);
        }

        public void SetSpinCount(int count, int maxCount)
        {
            spinCount.SetText($"<size=29>Match Remaining:</size>\n<font=Rigamesh SDF><size=41>{count}/{maxCount}</font>");
        }

        public void EnableGameComponents(bool enabled)
        {
            _components.SetActive(enabled);
            _gridUI.SetActive(enabled);
        }

        public void EnableOverlayBackground(bool enabled)
        {
            overlayBackground.enabled = enabled;
        }

        public void DisplayGridOverlayPopup(bool display, GridOverlayText type)
        {
            Animator animator;
            
            if (type == GridOverlayText.InsufficientMatch)
            {
                if (_insufficientMatchPopupDisplayed == display) return;
                
                animator = insufficientMatch;
                _insufficientMatchPopupDisplayed = display;
            }
            else
            {
                if (_startToPlayDisplayed == display) return;
                
                animator = _clickStartToPlayVFX;
                _startToPlayDisplayed = display;
            }

            animator.ResetTrigger(display ? "Off" : "On");
            animator.SetTrigger(display ? "On" : "Off");
            
            // Raycast receiver for triggering the spin event
            animator.transform.GetComponent<Image>().raycastTarget = display;
            
            _blinking.SetActive(display);
        }

        private void OnExperienceChanged()
        {
            var levelData = MainSceneController.Instance.Data.ExperienceData.Data;
            RewardEnum type = RewardEnum.GoldCoin;
            long amount = 0;
            if (MainSceneController.Instance.Data.ExperienceData.NextLevel != null)
            {
                foreach (var item in MainSceneController.Instance.Data.ExperienceData.NextLevel.RewardGain)
                {
                    if (item.Type == RewardEnum.GoldCoin.ToString())
                    {
                        type = RewardEnum.GoldCoin;
                        amount = item.Amount;
                    }
                    else if (item.Type == RewardEnum.StarCoin.ToString())
                    {
                        type = RewardEnum.StarCoin;
                        amount = item.Amount;
                    }
                    //else if (item.Type == RewardEnum.StarTicket.ToString())
                    //{
                    //    type = RewardEnum.StarTicket;
                    //    amount = item.Amount;
                    //}
                }
            }
            _levelProgressController.SetData(levelData.Level, levelData.Experience - levelData.BottomLevelUpExp, levelData.NextLevelUpExp - levelData.BottomLevelUpExp, type, amount);
        }

        private void OnLevelUp(int level)
        {
            var levelData = MainSceneController.Instance.Data.ExperienceData.Data;
            RewardEnum type = RewardEnum.GoldCoin;
            long amount = 0;
            if (MainSceneController.Instance.Data.ExperienceData.NextLevel != null)
            {
                foreach (var item in MainSceneController.Instance.Data.ExperienceData.NextLevel.RewardGain)
                {
                    if (item.Type == RewardEnum.GoldCoin.ToString())
                    {
                        type = RewardEnum.GoldCoin;
                        amount = item.Amount;
                    }
                    else if (item.Type == RewardEnum.StarCoin.ToString())
                    {
                        type = RewardEnum.StarCoin;
                        amount = item.Amount;
                    }
                    //else if (item.Type == RewardEnum.StarTicket.ToString())
                    //{
                    //    type = RewardEnum.StarTicket;
                    //    amount = item.Amount;
                    //}
                }
            }
            _levelUpController.SetData(levelData.Level, type, amount);
            _levelUpController.SetVisible(true);
        }

        private void OnMilestoneReached()
        {
            Debug.Log("OnMilestoneReached");
            var levelData = MainSceneController.Instance.Data.ExperienceData.Data;
            Debug.Log("Level Data in UI: " + JsonConvert.SerializeObject(levelData));
            Debug.Log("Milestone Data in UI: " + JsonConvert.SerializeObject(MainSceneController.Instance.Data.ExperienceData.NextMilestone));

            List<string> ids = new List<string>();
            if (MainSceneController.Instance.Data.ExperienceData.NextMilestone != null)
            {
                foreach (var item in MainSceneController.Instance.Data.ExperienceData.NextMilestone.RewardGain)
                {
                    if (item.Type != RewardEnum.GoldCoin.ToString() ||
                        item.Type != RewardEnum.StarCoin.ToString())
                    {
                        ids.Add(item.ItemId);
                    }
                }
            }
            
            Debug.Log("Ids in UI: " + JsonConvert.SerializeObject(ids));
            _milestoneController.SetData(levelData.Level, ids.ToArray());
            _milestoneController.SetVisible(true);
        }

        public void RegisterAllExperienceListeners()
        {
            _profileButton.onClick.AddListener(() =>
            {
                _levelProgressController.ToggleVisible();
                _levelUpController.SetVisible(false);
            });

            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.AddListener(OnExperienceChanged);
            MainSceneController.Instance.Data.ExperienceData.OnLevelUpChanged.AddListener(OnLevelUp);
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.AddListener(OnMilestoneReached);
            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.Invoke();
        }

        public void RemoveAllExperienceListeners()
        {
            MainSceneController.Instance.Data.ExperienceData.OnExperienceChanged.RemoveListener(OnExperienceChanged);
            MainSceneController.Instance.Data.ExperienceData.OnLevelUpChanged.RemoveListener(OnLevelUp);
            MainSceneController.Instance.Data.ExperienceData.OnMilestoneReached.RemoveListener(OnMilestoneReached);
        }

        public void RegisterGoToStoreButtonEvent(UnityAction gotoStoreAction)
        {
            _gotoStoreGoldButton.onClick.AddListener(gotoStoreAction);
            _gotoStoreStarButton.onClick.AddListener(gotoStoreAction);
        }

        public void RemoveGoToStoreButtonEvent()
        {
            _gotoStoreGoldButton.onClick.RemoveAllListeners();
            _gotoStoreStarButton.onClick.RemoveAllListeners();
        }

        public void SetButtonDisabledColor(bool enough)
        {
            _mainSpinButton.image.color = enough ? _mainSpinButton.colors.disabledColor : _mainSpinButton.colors.normalColor;
        }
    }
}