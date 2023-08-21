using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI
{
    public class PetLevelUpResultView : MonoBehaviour
    {
        [SerializeField] private List<PetLevelUpSubSkillView> _resultTexts;
        [SerializeField] private TextMeshProUGUI _levelTexts;
        [SerializeField] private ParticleSystem _resultVfx;
        [SerializeField] private Button _closeButton;
    
        public void DisplayResult(List<SubSkill> petPreviousSkills, List<SubSkill> petSubSkills, int dataLevelGain)
        {
            _resultTexts.ForEach(text => text.gameObject.SetActive(false));
            SetupData(petPreviousSkills, petSubSkills, dataLevelGain);
            GetComponent<FadeTween>().FadeIn();
            
            _resultVfx.Play();
            
            MainSceneController.Instance.Audio.PlaySfx(PetAudioKeys.SFX_PET010);
        }

        private void SetupData(List<SubSkill> petPreviousSkills, List<SubSkill> results, int level)
        {
            _levelTexts.SetText("Level " + level);

            for (int i = 0; i < results.Count; i++)
            {
                SubSkill previousSkill = null;
                SubSkill newSkill = results[i];

                if (petPreviousSkills != null)
                {
                    previousSkill = petPreviousSkills.Find(skill => skill.Id == results[i].Id);

                    if (previousSkill != null && previousSkill.Id == newSkill.Id && previousSkill.Value >= newSkill.Value)
                    {
                        _resultTexts[i].gameObject.SetActive(false);
                        continue;
                    }
                }
           
                _resultTexts[i].gameObject.SetActive(true);

                _resultTexts[i].SetupData(newSkill, previousSkill);
            }
        }

        public void RegisterOnCloseButton(UnityAction onClose)
        {
            _closeButton.onClick.AddListener(() =>
            {
                onClose();
            });
        }
    }
}
