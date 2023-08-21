using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI
{
    public class PetLevelProgressView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _progress;
        [SerializeField] TextMeshProUGUI _petLevelText;

        [SerializeField] private Slider _slider;

        public void ResetProgress()
        {
            _slider.minValue = 0;
            _slider.value = 0;
        }

        public Task DisplayLevelProgress(PetExperienceData petExperience, int maxLevel, bool isLevelUp = false)
        {
            var task = new TaskCompletionSource<bool>();
            _slider.minValue = petExperience.BottomExp;
            _slider.maxValue = petExperience.NextExp;
            
            _petLevelText.SetText("Lv. " + petExperience.Level);
            _progress.SetText(_slider.value + "/" + _slider.maxValue);

            if (isLevelUp)
            {
                LeanTween.value(_slider.value, petExperience.CurrentExp, Mathf.Abs(_slider.value - petExperience.CurrentExp) / 60f)
                    .setOnUpdate((val) =>
                    {
                        _slider.value = val;
                        _progress.SetText(_slider.value + "/" + _slider.maxValue);
                    })
                    .setOnComplete(() =>
                    {
                        if (petExperience.Level >= maxLevel)
                        {
                            DisplayMaxLevel();
                        }
                        task.SetResult(true);
                    });
            }
            else
            {
                _slider.value = petExperience.CurrentExp;
                _progress.SetText(_slider.value + "/" + _slider.maxValue);
                if (petExperience.Level >= maxLevel)
                {
                    DisplayMaxLevel();
                }
                task.SetResult(true);
            }
            
            
            return task.Task;
        }

        public async Task SetLevelProgressSequence(List<PetExperienceData> petExperiences, int maxLevel)
        {
            foreach (var experience in petExperiences)
            {
                await DisplayLevelProgress(experience, maxLevel, true);
            }
        }

        private void DisplayMaxLevel()
        {
            _slider.value = _slider.maxValue;
            _progress.SetText("Max Level");
        }
    }
}
