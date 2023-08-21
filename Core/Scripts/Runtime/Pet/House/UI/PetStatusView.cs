using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.ExtensionMethods;
using Starcade.Core.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI
{
    public class PetStatusView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Mandatory Fields")]
        [SerializeField] TextMeshProUGUI _petNameText;
        [SerializeField] TextMeshProUGUI _petBasicSkills;
        [SerializeField] List<TextMeshProUGUI> _petSubSkills;

        [Header("Optional Fields")]
        [SerializeField] TextMeshProUGUI _petLevelText;
        [SerializeField] Image _petImage;
        [SerializeField] GameObject _petActiveLabel;
        [SerializeField] private GameObject _noSubskillLabel;

        private Action _onClick;
        public void SetupView(PetInventoryData pet, Action onClick = null)
        {
            _petNameText.SetText(pet.Name);
            
            if (_petLevelText != null) _petLevelText.SetText("Lv. " + pet.ExperienceData.Level);
            
            if (_petImage != null)  _petImage.sprite = pet.GetImage();

            if(_petActiveLabel != null) _petActiveLabel.SetActive(pet.IsActive);
            
            _petBasicSkills.SetTextFromLocalizedId(PetSkillsLocalizations.Table, pet.BasicSkill.Type, new Dictionary<string, string>()
            {
                {"amount", pet.BasicSkill.Amount.ToString()}
            });
            
            
            SetupSubSkills(pet);

            _onClick = onClick;
            
            if(GetComponent<CanvasTransition>() != null) GetComponent<CanvasTransition>().TriggerTransition();
        }

        private void SetupSubSkills(PetInventoryData pet)
        {
            var subSkillAvailable = pet.SubSkills != null && pet.SubSkills.Count > 0;

            if(_noSubskillLabel != null) _noSubskillLabel.SetActive(subSkillAvailable == false);

            for (int i = 0; i < _petSubSkills.Count; i++)
            {
                if (i >= pet.SubSkills.Count)
                {
                    _petSubSkills[i].transform.parent.gameObject.SetActive(false);
                    continue;
                }

                ;

                _petSubSkills[i].transform.parent.gameObject.SetActive(true);
                var data = pet.SubSkills[i];

                if (data != null)
                {
                    var boxName =
                        MainSceneController.Instance.Localizations.GetLocalizedString(PetAdventureBoxLocalizations.Table,
                            data.Id);
                    _petSubSkills[i].SetTextFromLocalizedId(PetSkillsLocalizations.Table,
                        PetSkillsLocalizations.SubSkillsKey, PetSkillsLocalizations.GetSubSkillsParam(boxName, data.Value));
                }
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }
    }
}