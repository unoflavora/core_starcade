using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI
{
    public class PetLevelUpProgressView : MonoBehaviour
    {
        [SerializeField] private Slider _currentProgress;
        [SerializeField] private Slider _progressBar;

        [Header("Level Progress")] [SerializeField]
        private TextMeshProUGUI _currentLevelText;

        [SerializeField] private TextMeshProUGUI _gainLevelText;
        [SerializeField] private GameObject _arrowLevel;

        [Header("Experience Progress")] 
        [SerializeField] private TextMeshProUGUI _currentExp;
        [SerializeField] private TextMeshProUGUI _gainExp;

        private PetLevelUpConfig _config;
        private PetExperienceData _petExperienceData;
        
        
        public void SetPetCurrentProgress(PetExperienceData experienceData, PetLevelUpConfig levelUpConfig)
        {
            _currentProgress.minValue = 0;
            _currentProgress.maxValue = experienceData.NextExp;
            _currentProgress.value = experienceData.CurrentExp;
            
            _progressBar.minValue = _currentProgress.minValue;
            _progressBar.maxValue = _currentProgress.maxValue;
            _progressBar.value = _currentProgress.value;

            _currentLevelText.SetText("Lv. " + experienceData.Level);
            _currentExp.SetText(experienceData.CurrentExp + " / " + experienceData.NextExp);

            _gainExp.gameObject.SetActive(false);
            _gainLevelText.gameObject.SetActive(false);
            _arrowLevel.gameObject.SetActive(false);

            _config = levelUpConfig;
            _petExperienceData = experienceData;
        }

        public int SetPetProgress(List<CollectibleItem> items)
        {
            if (_config == null) return 0;
            
            var totalCollectibleExp = 0;
        
            foreach(var item in items)
            {
                var rarity = item.Rarity;
            
                var exp = _config.ExpGainFormulas.Find(formula => formula.CollectibleRarity == rarity);
            
                totalCollectibleExp += exp.ExpAmount;
            }

            _gainExp.gameObject.SetActive(totalCollectibleExp > 0);
            
            return SetProgressGain(totalCollectibleExp);
        }

        private int SetProgressGain(int totalCollectibleExp)
        {
            _gainExp.gameObject.SetActive(totalCollectibleExp > 0);

            var (totalExp, levelGain, nextLevel, nextExpLevel) = GetTotalExp(totalCollectibleExp, 0, _petExperienceData.Level + 1, _petExperienceData.NextExp);

            _progressBar.value = totalExp;
            
            _gainExp.SetText("+" + totalCollectibleExp);
            
            _currentExp.SetText(totalExp + " / " + nextExpLevel);
            
            if (totalExp >= _petExperienceData.NextExp)
            {
                _gainLevelText.SetText("Lv. " + (_petExperienceData.Level + levelGain));
                _gainLevelText.gameObject.SetActive(true);
                _arrowLevel.gameObject.SetActive(true);
                
                if (_petExperienceData.Level + levelGain >= _config.MaxLevel)
                {
                    _currentExp.SetText("Max Level");
                    return nextLevel;
                }
            }
            else
            {
                _gainLevelText.gameObject.SetActive(false);
                _arrowLevel.gameObject.SetActive(false);
            }

            return _petExperienceData.Level + levelGain;
        }

        private (int, int, int, int) GetTotalExp(int totalCollectibleExp, int levelGain, int nextLevel, int nextLevelExp)
        {
            var totalExp = _petExperienceData.CurrentExp + totalCollectibleExp;
            
            if (_petExperienceData.Level + levelGain >= _config.MaxLevel || totalCollectibleExp == 0)
            {
                return (totalExp, levelGain, nextLevel, nextLevelExp);
            }
            
            if (totalExp >= nextLevelExp)
            {
                levelGain++;
            
                nextLevel++;
            
                nextLevelExp = PetLevelUpCalculator.PetLevelUpExpCalculator.GetTotalExpAtLevel(nextLevel, _config.LevelUpFormula);
                
                return GetTotalExp(totalCollectibleExp, levelGain, nextLevel, nextLevelExp);
            }

            return (totalExp, levelGain, nextLevel, nextLevelExp);
        }
    }
}
