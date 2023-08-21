using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.UI;
using Agate.Starcade.Core.Runtime.Pet.Core.Backend;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.Core.Popups;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.Modules
{
    public class PetHouseStatusController : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private InteractablePetStatusView _petStatusView;
        [SerializeField] private PetLevelProgressView _progressView;

        [FormerlySerializedAs("_clickButton")]
        [Header("Interactables")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _sendGiftButton;
        [SerializeField] private Button _levelUpClick;
        
        [Header("Popups")]
        [SerializeField] private GameObject _overlay;
        [SerializeField] private PinGiftFriendListController _giftFriendListController;
        [SerializeField] private PetLevelUpController _levelUpController;
        [SerializeField] private PetConfirmGiftView _confirmGiftView;
        
        [Header("VFX")] 
        [SerializeField] private GameObject _blocker;
        [SerializeField] private ParticleSystem _levelUpVfx;
        [SerializeField] private ParticleSystem _expUpVfx;

        private PetInventoryData _currentViewedPet;
        private PetHouseAnalyticEventHandler _analytic;
        
        private static PetLevelUpConfig _petConfigsLevelUpConfig => MainSceneController.Instance.Data.PetConfigs.LevelUpConfig;
        private static List<PetInventoryData> _petInventoryData => MainSceneController.Instance.Data.PetInventory;
        private void Start()
        {
            InitInteractions();
        }

        public void SetupAnalytic(PetHouseAnalyticEventHandler analyticEventHandler)
        {
            _analytic = analyticEventHandler;
        }

        public void SetPetStatus(PetInventoryData pet)
        {
            SetDisplayedPet(pet);
            _progressView.ResetProgress();
            _progressView.DisplayLevelProgress(pet.ExperienceData, _petConfigsLevelUpConfig.MaxLevel);
            _levelUpController.SetAnalytic(_analytic);
        }

        public void RegisterOnCloseClicked(UnityAction onClose)
        {
            _backButton.onClick.AddListener(() =>
            {
                _overlay.SetActive(false);
                onClose();
            });
        }

        private void InitInteractions()
        {
            _levelUpController.RegisterOnCloseLevelUp(OnUserFinishLevelUpInteraction);
            
            _levelUpClick.onClick.AddListener(() =>
            {
                _overlay.SetActive(true);
                _levelUpController.SetPet(_currentViewedPet);
                _levelUpController.gameObject.SetActive(true);
                _analytic.TrackClickPetHouseItemLevelUpButtonEvent(_currentViewedPet.Id);
            });

            _sendGiftButton.onClick.AddListener(() =>
            {
                _analytic.TrackClickPetHouseItemGivePetButtonEvent(_currentViewedPet.Id);
                ShowSendPetPopup();
            });
        }

        private void ShowSendPetPopup()
        {
            _overlay.SetActive(true);
            _giftFriendListController.gameObject.SetActive(true);
            _giftFriendListController.DisplayFriends(DisplaySendPetConfirmation, () => _overlay.SetActive(false));
        }

        private void DisplaySendPetConfirmation(FriendProfile friendProfile)
        {
            _overlay.SetActive(true);
            
            _confirmGiftView.DisplayConfirmation(friendProfile.Username, _currentViewedPet, async () =>
            {
                var giftPet = await PetBackendController.Instance.GiftPet(friendProfile.FriendCode, _currentViewedPet.UniqueId);
                if (giftPet.Error != null)
                {
                    MainSceneController.Instance.Info.ShowSomethingWrong(giftPet.Error.Code);
                    return;
                }
                
                MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET007);
                
                MainSceneController.Instance.Data.PetInventory.RemoveAt(_petInventoryData.FindIndex(i => i.UniqueId == giftPet.Data.PetUniqueId));
                
                MainSceneController.Instance.Info.Show("You have successfully gifted your pet to your friend", "Pet Gifted", InfoIconTypeEnum.Success, new InfoAction("Close", _backButton.onClick.Invoke), null);
                
                _analytic.TrackGivePetEvent(new PetHouseAnalyticEventHandler.GivePetParameterData()
                {
                    PetId = _currentViewedPet.Id,
                    PetLevel = _currentViewedPet.ExperienceData.Level,
                    PetUniqueId = _currentViewedPet.UniqueId,
                    SubSkills = _currentViewedPet.SubSkills.Select(skill => new PetHouseAnalyticEventHandler.SkillParameterData()
                    {
                        Id = skill.Id,
                        Value = (float) skill.Value
                    } ).ToList()
                });
            }, () =>
            {
                ShowSendPetPopup();
            });
            
            _confirmGiftView.gameObject.SetActive(true);
            
        }

        private async void OnUserFinishLevelUpInteraction(ExperienceStatus levelUpStatus, PetLevelUpData levelUpData)
        {
            _overlay.gameObject.SetActive(false);
            _levelUpController.gameObject.SetActive(false);

            if (levelUpStatus == ExperienceStatus.None) return;

            _petStatusView.ActivatePet();
            
            _blocker.SetActive(true);
            
            
            await Task.WhenAll(
                _progressView.SetLevelProgressSequence
                (_levelUpController.GetProgressSequence(levelUpData.ExpGain), _petConfigsLevelUpConfig.MaxLevel),
                PlayVFX(levelUpStatus));

            var petIndex = MainSceneController.Instance.Data.PetInventory.FindIndex(i => i.UniqueId == levelUpData.Pet.UniqueId);
            if (levelUpStatus == ExperienceStatus.LevelUp)
            {
                _overlay.gameObject.SetActive(true);
                _levelUpController.gameObject.SetActive(true);
                _levelUpController.ShowLevelUpPopup(MainSceneController.Instance.Data.PetInventory[petIndex], levelUpData);
            }
            
            _blocker.SetActive(false);
            
            var updatedPet = levelUpData.Pet;
            
            _petStatusView.UpdatePetData(updatedPet);

            MainSceneController.Instance.Data.PetInventory[petIndex] = updatedPet;
            
			SetupButtonsInteractable(updatedPet);
        }

        private void SetDisplayedPet(PetInventoryData pet)
        {
            _petStatusView.SetupView(pet, _analytic);
            
            _currentViewedPet = pet;

            SetupButtonsInteractable(pet);
        }

        private void SetupButtonsInteractable(PetInventoryData pet)
        {
			_levelUpClick.interactable = pet.ExperienceData.Level < _petConfigsLevelUpConfig.MaxLevel;

			_sendGiftButton.interactable = pet.IsActive == false;
		}


        private async Task PlayVFX(ExperienceStatus petExperienceStatus)
        {
            // Disable click when vfx is playing
            switch (petExperienceStatus)
            {
                case ExperienceStatus.ExpUp:
                    _expUpVfx.Play();
                    break;
                case ExperienceStatus.LevelUp:
                    _levelUpVfx.Play();
                    break;
            }

            await AsyncUtility.WaitUntil(() => _expUpVfx.isStopped && _levelUpVfx.isStopped);
        }
    }
}