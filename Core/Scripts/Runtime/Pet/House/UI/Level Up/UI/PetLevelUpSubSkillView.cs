using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Starcade.Core.ExtensionMethods;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.House.UI.Level_Up.UI
{
    public class PetLevelUpSubSkillView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _previousSkill;

        [SerializeField] private TextMeshProUGUI _updatedSkill;

        [SerializeField] private GameObject _arrow;

        public void SetupData(SubSkill updatedSkill, SubSkill previousSkill = null)
        {
            _previousSkill.gameObject.SetActive(previousSkill != null);
            _arrow.SetActive(previousSkill != null);
        
            if (previousSkill != null)
            {
                _previousSkill.SetTextAsSubSkill(previousSkill.Id, previousSkill.Value);
            
                // Increase value
                _updatedSkill.SetText(updatedSkill.Value + "%");
            }
            else
            {
                _updatedSkill.SetTextAsSubSkill(updatedSkill.Id, updatedSkill.Value);
            }
        
        }
    }
}
