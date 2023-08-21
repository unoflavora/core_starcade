using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Analytics.Handlers.Pet;
using Agate.Starcade.Core.Runtime.Pet.Core;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up;
using Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI;
using Agate.Starcade.Core.Runtime.Pet.Core.Backend;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using Agate.Starcade.Scripts.Runtime.Collectibles.Data;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI.Slot;
using Agate.Starcade.Scripts.Runtime.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;
using CollectibleSetData = Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles.CollectibleSetData;
using Newtonsoft.Json;

namespace Agate.Starcade.Core.Runtime.Pet.House.Modules
{
    public class PetLevelUpController : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private PetLevelUpPreviewGridController _preview;
        [FormerlySerializedAs("_resultView")] [SerializeField] private PetLevelUpResultView _levelUpResultView;

        [Header("Interactables")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _levelupButton;
        [SerializeField] private TMP_Dropdown _dropdown;
        
        [Header("UI and Effects")]
        [SerializeField] private PoolableGridUI _grid;
        [SerializeField] private GameObject _inavailableText;
        [SerializeField] private Animator _animationPrepab;

        private List<CollectibleItem> _data;
        private List<string> _setNames;
        
        private MainModel _mainModel;
        private PetInventoryData _petData;
        
        private UnityAction<ExperienceStatus, PetLevelUpData> _onPetExperienceChanged;
        private PetLevelUpConfig _config;
        private PetHouseAnalyticEventHandler _analytic;
        
        private Stack<Animator> _animations;

        private int _levelProgress;
        private bool _isLevelUpMaxed => _levelProgress >= _config.MaxLevel;

        private void Start()
        {
            _animations = new Stack<Animator>();
            
            _closeButton.onClick.AddListener(() => _onPetExperienceChanged.Invoke(ExperienceStatus.None, null));
            
            _levelupButton.onClick.AddListener(OnConfirmLevelUp);
            
            _dropdown.onValueChanged.AddListener(OnFilterChanged);
            
            _levelUpResultView.RegisterOnCloseButton(() =>
            {
                _levelUpResultView.gameObject.SetActive(false);
                _preview.gameObject.SetActive(false);
                _closeButton.onClick.Invoke();
            });
            
            _levelupButton.interactable = false;
            
            _config = MainSceneController.Instance.Data.PetConfigs.LevelUpConfig;
        }

        private void OnFilterChanged(int setIndex)
        {
            DisplayAvailableCollectiblesForLevelUp(_setNames[setIndex]);
        }

        void OnEnable()
        {
            _mainModel = MainSceneController.Instance.Data;
            
            // Update Pet Data
            _petData = _mainModel.PetInventory.Find(pet => pet.UniqueId == _petData.UniqueId);
            
            DisplayAvailableCollectiblesForLevelUp();
            
            _preview.SetProgressConfig(_petData.ExperienceData, _mainModel.PetConfigs.LevelUpConfig);
            
            _levelupButton.interactable = false;
        }

        private async void DisplayAvailableCollectiblesForLevelUp(string setName = null)
        {
            _setNames = new List<string>();

            GetCollectibleItems(setName);

            foreach (var set in _mainModel.CollectiblesData)
            {
                if(set.IsComingSoon) continue;
                _setNames.Add(set.CollectibleSetName);
            }
            
            _dropdown.options.Clear();
            foreach (var option in _setNames)
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
            
            DrawGrid();
            
            _inavailableText.SetActive(_data.Count < 1);
            
            _preview.gameObject.SetActive(true);
        }

        private void GetCollectibleItems(string setName)
        {
            _data = new List<CollectibleItem>();
            
            CollectibleSetData currentSet = new CollectibleSetData();
            for (int i = 0; i < _mainModel.CollectiblesData.Count; i++)
            {
                var set = _mainModel.CollectiblesData[i];
                if (!set.IsComingSoon)
                {
                    currentSet = set;
                }

                if (set.CollectibleSetName == setName)
                {
                    currentSet = set;
                    break;
                }
                
            }
            var items = currentSet.CollectibleItems.Where((t => t.Amount > 1)).ToList();
            
            foreach (var item in items)
            {
                _data.Add(new CollectibleItem()
                {
                    Rarity = item.Rarity,
                    Amount = item.Amount - 1,
                    CollectibleItemName = item.CollectibleItemName,
                    CollectibleItemId =item.CollectibleItemId
                });
            }
        }

        private void OnDisable()
        {
            _preview.Clear();
        }

        public void RegisterOnCloseLevelUp(UnityAction<ExperienceStatus, PetLevelUpData> onClose)
        {
            _onPetExperienceChanged += onClose;
        }

        public void SetAnalytic(PetHouseAnalyticEventHandler analyticEventHandler)
        {
            _analytic = analyticEventHandler;
        }

        private void DrawGrid()
        {
            _data = _data.OrderByDescending(item => item.Rarity).ToList();
            _grid.Draw(_data, OnItemClicked);
        }

        private void OnItemClicked(Transform clickedItem)
        {
            if (_isLevelUpMaxed) return;
            
            var collectible = clickedItem.GetComponent<CollectibleSlot>();

            var enoughCapacity = _preview.AddItem(collectible.ItemData, OnCancelItem);
            
            if (enoughCapacity)
            {
                _levelProgress = _preview.UpdateProgress();
                
                UpdateButtonState();
                
                PlayFlyingLoveVFX(clickedItem);
                
                MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET008);

                
                collectible.ItemData.Amount -= 1;

                if (collectible.ItemData.Amount == 0)
                {
                    _data.RemoveAt(_data.FindIndex(data => data.CollectibleItemId == collectible.ItemData.CollectibleItemId));
                }

                DrawGrid();
            }
        }

        private async void PlayFlyingLoveVFX(Transform loc)
        {
            Animator anim;
            
            anim = _animations.Count > 0 ? _animations.Pop() : Instantiate(_animationPrepab, transform);
            
            anim.gameObject.transform.position = loc.position;
            
            await Play(anim);
        }

        private async Task Play(Animator anim)
        {
            anim.gameObject.SetActive(true);
            anim.Play("A_ConsumePin_Love");
            await Task.Delay(1500); 
            
            anim.gameObject.SetActive(false);
            _animations.Push(anim);
        }

        private void UpdateButtonState()
        {
            _levelupButton.interactable = _preview.SelectedItems.Count > 0;
        }


        private void OnCancelItem(CollectibleItem item)
        {
            // var collectible = _grid.ItemObjects[_grid.ItemObjects.FindIndex(i => i.GetComponent<CollectibleSlot>().ItemData.CollectibleItemId == item.CollectibleItemId)];
            
            if (item.Amount == 0) _data.Add(item);

            item.Amount += 1;

            DrawGrid();
            
            _levelProgress = _preview.UpdateProgress();
            
            UpdateButtonState();
        }

        private async void OnConfirmLevelUp()
        {
            var itemIds = _preview.SelectedItems.Select(item => item.CollectibleItemId).ToList();
            
            var levelUpResponse = await PetBackendController.Instance.LevelUp(_petData.UniqueId,itemIds);
            if (levelUpResponse.Error != null)
            {
                MainSceneController.Instance.Info.ShowSomethingWrong(levelUpResponse.Error.Code);
                return;
            }

            _analytic.TrackLevelUpPetEvent(new PetHouseAnalyticEventHandler.LevelUpPetParameterData()
            {
                CurrentLevel = _petData.ExperienceData.Level,
                TargetLevel = levelUpResponse.Data.Pet.ExperienceData.Level,
                PetId = _petData.Id,
                PetUniqueId = _petData.UniqueId,
                Pins = _preview.SelectedItems.Select(item => new PetHouseAnalyticEventHandler.PinParameterData()
                {
                    Id = item.CollectibleItemId,
                    Rarity = item.Rarity
                }).ToList()
            });
            
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET009);

            foreach (var itemId in itemIds)
            {
                CollectiblesController.UpdateCollectibleAmountInModel(itemId, -1);
            }
            
            if (levelUpResponse.Data.LevelGain > 0)
            {
                _onPetExperienceChanged.Invoke(ExperienceStatus.LevelUp, levelUpResponse.Data);
            }
            else
            {
                _onPetExperienceChanged.Invoke(ExperienceStatus.ExpUp, levelUpResponse.Data);
            }
        }


        public void ShowLevelUpPopup(PetInventoryData previousPetData, PetLevelUpData data)
        {
            _preview.SetProgressConfig(data.Pet.ExperienceData, _mainModel.PetConfigs.LevelUpConfig);

            _preview.gameObject.SetActive(false);
            
            _levelUpResultView.DisplayResult(previousPetData.SubSkills, data.Pet.SubSkills, data.Pet.ExperienceData.Level);
            
            _levelUpResultView.gameObject.SetActive(true);
        }

        public void SetPet(PetInventoryData data)
        {
            _petData = data;
            
            _levelProgress = _petData.ExperienceData.Level;
        }
        
        // TODO move this into some controller so that it can be used in PetLevelUpProgressView.cs
        public List<PetExperienceData> GetProgressSequence(int totalCollectibleExp)
        {
            List<PetExperienceData> experiences = new List<PetExperienceData>();
            
            var previousExp = _petData.ExperienceData.CurrentExp;
            
            var totalExp = _petData.ExperienceData.CurrentExp + totalCollectibleExp;
            
            var nextLevel = _petData.ExperienceData.Level + 1;

            var levelGain = 0;
            
            var nextLevelExp = PetLevelUpCalculator.PetLevelUpExpCalculator.GetTotalExpAtLevel(nextLevel, _config.LevelUpFormula);
            
            experiences.Add(new PetExperienceData()
            {
                BottomExp = previousExp,
                CurrentExp = nextLevelExp,
                Level = _petData.ExperienceData.Level,
                NextExp = nextLevelExp
            });
            
            while (totalExp >= nextLevelExp)
            {
                nextLevel++;
                levelGain++;
                previousExp = nextLevelExp;
                nextLevelExp =
                    PetLevelUpCalculator.PetLevelUpExpCalculator.GetTotalExpAtLevel(nextLevel, _config.LevelUpFormula);
                
                experiences.Add(new PetExperienceData()
                {
                    BottomExp = previousExp,
                    CurrentExp = nextLevelExp,
                    Level = _petData.ExperienceData.Level + levelGain,
                    NextExp = nextLevelExp
                });
            }

            experiences[experiences.Count - 1].CurrentExp = totalExp;

            return experiences;
        }

        
    }
    
    public enum ExperienceStatus {ExpUp, LevelUp, None}
}
