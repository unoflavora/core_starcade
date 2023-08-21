using System;
using System.Collections.Generic;
using System.Linq;
using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.MyPet.Interaction;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI;
using Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Popups;
using Agate.Starcade.Core.Runtime.Pet.Core.Backend;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Scripts.Runtime.Utilities;
using IngameDebugConsole;
using Starcade.Core.Localizations;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet
{
    public class MyPetSceneController : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private MyPetUIController _uiController;
        [SerializeField] private PetInteractionController _petInteractionController;
        [SerializeField] private PetPopupsController _petPopupsController;
        private PetBackendController _backend;
        
        [Header("Pet Config")]
        [SerializeField] private PetSpineController _currentPetView;
        private PetInventoryData _currentPet;
        private List<PetInventoryData> _petInventory;

        [Header("Pet Development Config")]
        private bool _isDevelopmentMode;
        private MainModel _mainModel;
        private MyPetAnalyticEventHandler _analytic;
        private PetAdventureAnalyticEventHandler _adventureAnalytic;
        private string _currentAdventureConfigId;

        [Header("Visual Effects")]
        [SerializeField] private AnimationEventHandler _dispatchVfx;
        [SerializeField] private ParticleSystem _glowVfx;
        [SerializeField] private PetSpineController _dispatchedPet;
        
        private void Start()
        {
            _mainModel = MainSceneController.Instance.Data;
            _backend = PetBackendController.Instance;
            _analytic = new MyPetAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _adventureAnalytic = new PetAdventureAnalyticEventHandler(MainSceneController.Instance.Analytic);
            _isDevelopmentMode = _mainModel.PetConfigs == null;
            if (_isDevelopmentMode) UseDummyData();
            
            AddCheats();
            InitListeners();
            InitPets();
            ResetPetInteraction();
            LoadAudio();
            
            _uiController.GetComponent<CanvasTransition>().TriggerTransition();
            
            MainSceneController.Instance.Loading.DoneLoading();
        }

        private async void LoadAudio()
        {
            await MainSceneController.Instance.Audio.LoadAudioData("mypet_audio");

            MainSceneController.Instance.Audio.PlayBgm(PetAudioKeys.AMB_PET001);
        }

        private void InitListeners()
        {
            
            _uiController.RegisterOnChangePetClick(() =>
            {
                _analytic.TrackClickSwitchPetButtonEvent();
                
                if (_mainModel.PetAdventureData.IsDispatched)
                {
                    _uiController.DisplayErrorOnChangePet("You can’t switch pets while your active pet is still exploring.");
                }
                else
                {
                    if (_mainModel.PetAdventureData.Rewards != null)
                    {
                        _uiController.DisplayErrorOnChangePet("You can’t switch pet while there there is unclaimed reward");
                        return;
                    }
                    
                    _petPopupsController.OpenChangePetPopup(MainSceneController.Instance.Data.PetInventory, OnChangePet);
                }

            });
            
            _uiController.RegisterOnCancelAdventure(OnCancelPetAdventure);
            _uiController.RegisterOnClaimAdventureReward(OnPetClaimRewardAdventure);
            _uiController.RegisterOnAdventureClick(DisplayPetAdventureOptions);
            _uiController.RegisterOnAdventureFinished(OnAdventureFinished);
            
            _dispatchVfx.RegisterOnAnimationTrigger((string eventKey) =>
            {
                switch (eventKey)
                {
                    case "Shoot":
                        _glowVfx.Play();
                        break;
                    case "Finish":
                        _dispatchVfx.gameObject.SetActive(false);
                        break;
                }
            });
        }
        


        private async void OnAdventureFinished()
        {
            var refreshedData = await MainSceneController.Instance.GameBackend.GetPetsLibraryData();
            
            if (refreshedData.Error == null)
            {
                _mainModel.PetAdventureData = refreshedData.Data.AdventureData;
                _uiController.SetCurrentActivity(_currentPet.Name, _mainModel.PetAdventureData);
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
        }

        private void DisplayPetAdventureOptions()
        {
            _analytic.TrackClickAdventureButtonEvent();
            _petPopupsController.OpenChooseAdventurePopup(_currentPet, _mainModel.PetConfigs.AdventureConfigs, OnPetDispatch);
        }
        
        private void OnEnable()
        {
            InitPets();
        }

        private void OnDisable()
        {
            MainSceneController.Instance.Audio.PlayBgm(MainSceneController.AUDIO_KEY.BGM_LOBBY);
        }

        private void AddCheats()
        {
            DebugLogConsole.AddCommand("skip_adventure", "Skip Adventure", async () =>
            {
                await _backend.SkipAdventure();
                
                OnAdventureFinished();
            });

            DebugLogConsole.AddCommand<string>("add_pet", "Add Dummy Pet", async (petId) =>
            {
                await _backend.AddPet(petId);

                var petsResponse = await RequestHandler.Request(async () =>
                    await MainSceneController.Instance.GameBackend.GetPetsLibraryData());

                if (petsResponse.Error != null)
                {
                    throw new Exception(petsResponse.Error.Code);
                }

                MainSceneController.Instance.Data.PetInventory = petsResponse.Data.PetInventory;
                MainSceneController.Instance.Data.PetAlbum = petsResponse.Data.Pets;
                MainSceneController.Instance.Data.PetFragment.Inventory = petsResponse.Data.PetFragmentInventory;
                MainSceneController.Instance.Data.PetConfigs = petsResponse.Data.PetConfigs;
                MainSceneController.Instance.Data.PetAdventureData = petsResponse.Data.AdventureData;
            });
        }

        private void InitPets()
        {
            if(_mainModel == null) return;

            _petInventory = _mainModel.PetInventory;
            
            if (_petInventory.Count < 1)
            {
                // use index 0 as a default pet
                _uiController.DisplaySilhouette(true, _mainModel.PetAlbum[0].GetImage(true));
                return;
            }

            _uiController.DisplaySilhouette(false);
            var petData = _petInventory.Find(pet => pet.IsActive);
            DisplayPetInScene(petData);
        }

        private async void OnChangePet(PetInventoryData pet)
        {
            if (pet == _currentPet) return;
            
            var data = await _backend.SwitchPet(pet.UniqueId);

            if (data.Error == null)
            {
                _petInventory[_petInventory.FindIndex(petData => petData.UniqueId == pet.UniqueId)].IsActive = true;
                
                _petInventory[_petInventory.FindIndex(petData => petData.UniqueId == _currentPet.UniqueId)].IsActive = false;
                                
                ResetPetInteraction();

                DisplayPetInScene(pet);

                _analytic.TrackSwitchPetEvent(pet.Id, pet.UniqueId);
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
        }


        private async void OnPetDispatch(int val)
        {
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET005);

            var config = _mainModel.PetConfigs.AdventureConfigs.Find(config => config.Time == val);
            
            _currentAdventureConfigId = config.Id;
            
            var dispatch = await _backend.Dispatch(config.Id);

            _dispatchVfx.gameObject.SetActive(true);
            
            _dispatchVfx.PlayAnimation("A_PetGoing");
            
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET004);

            if (dispatch.Error == null)
            {
                MainSceneController.Instance.Data.PetAdventureData = dispatch.Data;
                _adventureAnalytic.TrackDispatchPetAdventureEvent(new PetAdventureAnalyticEventHandler.DispatchPetData()
                {
                    BasicSkill = new PetAdventureAnalyticEventHandler.SkillData()
                    {
                        Id = _currentPet.BasicSkill.Type,
                        Value = _currentPet.BasicSkill.Amount
                    },
                    ConfigId = config.Id,
                    PetId = _currentPet.Id,
                    PetUniqueId = _currentPet.UniqueId,
                    PetLevel = _currentPet.ExperienceData.Level,
                    SessionId = dispatch.Data.AdventureSessionId,
                    SubSkills = _currentPet.SubSkills.Select(skill => new PetAdventureAnalyticEventHandler.SkillData
                    {
                        Id = skill.Id,
                        Value = (float) skill.Value
                    }).ToList()
                });
                _uiController.SetCurrentActivity(_currentPet.Name, _mainModel.PetAdventureData);
            }
            else
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(dispatch.Error.Code);
            }
        }

        private void OnCancelPetAdventure()
        {
            _analytic.TrackClickRecallButtonEvent();

            MainSceneController.Instance.Info.Show("You will lose all your progress if you recall your pet", "Recall Pet", InfoIconTypeEnum.Alert, new InfoAction("Confirm", async () =>
            {
                var dispatch = await _backend.CancelDispatch();

                if (dispatch.Error == null)
                {
                    MainSceneController.Instance.Data.PetAdventureData = dispatch.Data.AdventureData;
                    _uiController.SetCurrentActivity(_currentPet.Name, _mainModel.PetAdventureData);
                    
                    _adventureAnalytic.TrackRecallPetAdventureEvent(new PetAdventureAnalyticEventHandler.BaseAdventureData()
                    {
                        ConfigId = _currentAdventureConfigId,
                        PetId = _currentPet.Id,
                        PetUniqueId = _currentPet.UniqueId,
                        SessionId = dispatch.Data.AdventureData.AdventureSessionId,
                    });
                }
                else
                {
                    if (dispatch.Error.Code == "28007")
                    {
                        MainSceneController.Instance.Info.Show("", "Your Pet has returned from Adventure", InfoIconTypeEnum.Alert, new InfoAction()
                        {
                            Action = OnAdventureFinished,
                            ActionName = "Close"
                        }, null);
                        return;   
                    }
                    
                    MainSceneController.Instance.Info.ShowSomethingWrong(dispatch.Error.Code);
                }
            }), new InfoAction("Cancel", () => {}));
        }

        public async void OnPetClaimRewardAdventure()
        {
            var dispatch = await _backend.ClaimReward();

            if (dispatch.Error == null)
            {
                MainSceneController.Instance.Data.PetAdventureData.Rewards = null;
                _uiController.SetCurrentActivity(_currentPet.Name, _mainModel.PetAdventureData);
                
                Sprite[] assets = new Sprite[dispatch.Data.Count];
                String[] descs = new String[dispatch.Data.Count];
                
                MainSceneController.Instance.Data.ProcessRewards(dispatch.Data.ToArray());
                for (int i = 0; i < dispatch.Data.Count; i++)
                {
                    assets[i] = MainSceneController.Instance.AssetLibrary.GetSpriteRewardAsset(dispatch.Data[i]);
                    descs[i] = $"{CurrencyHandler.Convert(dispatch.Data[i].Amount)} {dispatch.Data[i].GetRewardName()}";
                }

                MainSceneController.Instance.Info.ShowReward("CONGRATULATIONS", "Pet Adventure Reward", assets, descs, new InfoAction()
                {
                    Action = () => { },
                    ActionName = "Close"
                }, null);
                
                MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET007);

                _adventureAnalytic.TrackClaimCompletePetAdventureEvent(new PetAdventureAnalyticEventHandler.ClaimCompleteAdventureData()
                {
                    ConfigId = _currentAdventureConfigId,
                    PetId = _currentPet.Id,
                    PetLevel = _currentPet.ExperienceData.Level.ToString(),
                    PetUniqueId = _currentPet.UniqueId,
                    Rewards = dispatch.Data.Select(reward => new PetAdventureAnalyticEventHandler.RewardData()
                    {
                        Type = reward.Type.ToString(),
                        Amount = reward.Amount
                    }).ToList(),
                    SessionId = MainSceneController.Instance.Data.PetAdventureData.AdventureSessionId
                });
            }
            else
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(dispatch.Error.Code);
            }
            
            MainSceneController.Instance.Loading.DoneLoading();
        }

        private void DisplayPetInScene(PetInventoryData pet)
        {
            if (_petInventory.Count > 0)
            {
                var spineData = MainSceneController.Instance.AssetLibrary.GetPetObject(pet.Id).SkeletonDataAsset;
                
                _currentPetView.SetPetAsset(spineData);
                
                _dispatchedPet.SetPetAsset(spineData);

                _uiController.SetPetData(pet, _mainModel.PetConfigs.LevelUpConfig.MaxLevel);
                _uiController.SetCurrentActivity(pet.Name, _mainModel.PetAdventureData);
                _uiController.SetUpPetStatus(pet);
                _currentPet = pet;
                _currentPet.IsActive = true;
            }
        }
        
        public void OnPetStatusClicked()
        {
            _analytic.TrackClickPetStatusButtonEvent();
        }
        
        private void ResetPetInteraction()
        {
            _petInteractionController.Handle(_currentPetView);
            
            _uiController.ResetTooltip();
            
            _petInteractionController.RegisterOnPetInteract(() =>
            {
                _uiController.NextTooltip();
                _analytic.TrackTouchPetEvent();
            });
        }

        /// <summary>
        /// DEVELOPMENT ONLY
        /// </summary>
        private async void UseDummyData()
        {
            var data = _backend.GetDummyPetData();
            _mainModel.PetInventory = data.PetInventory;
            _mainModel.PetConfigs = data.PetConfigs;
            _mainModel.PetAlbum = data.Pets;

            await MainSceneController.Instance.AssetLibrary.LoadAllAssets();
        }

        private void OnDestroy()
        {
            DebugLogConsole.RemoveCommand("skip_adventure");
            DebugLogConsole.RemoveCommand("add_pet");
        }
    }
}
