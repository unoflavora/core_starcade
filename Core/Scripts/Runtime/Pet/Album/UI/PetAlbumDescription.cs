using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.MyPet.Interaction;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Runtime.Main;
using Starcade.Core.ExtensionMethods;
using Starcade.Core.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Album.UI
{
    public class PetAlbumDescription : MonoBehaviour
    {
        [Header("Interactables")] 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _obtainPet;

        [Header("Basic Skills")]
        [SerializeField] private TextMeshProUGUI _basicSkill;
        [SerializeField] private Image _basicSkillInactiveBackground;
        [SerializeField] private Image _basicSkillBackground;

        [Header("Pet Profiles")]
        [SerializeField] private Image _petImage;
        [SerializeField] private PetSpineController _petSpine;
        [SerializeField] private TextMeshProUGUI _petName;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _petNameBackground;
        [SerializeField] private Color _petNameBackgroundColor;
        
        [Header("UI")]
        [SerializeField] private GameObject _unavailablePet;
        [SerializeField] private Image _descriptionBackground;
        [SerializeField] private Color _silhouetteColor;
        [SerializeField] private GameObject _petStar;

        private StringTable _englishSkill;

        public void DisplayPet(PetAlbumData data, UnityAction onBackButton, UnityAction onObtainPet)
        {
            bool petIsOwned = data.HasOwned;

            InitListeners(onBackButton, onObtainPet);
            SetupBasicSkillDisplay(data, petIsOwned);
            SetupPetProfile(data, petIsOwned);
            SetupPetDescription(data, petIsOwned);
            SetupPetProfile(petIsOwned);
            
            gameObject.SetActive(true);
            
            GetComponent<CanvasTransition>().TriggerTransition();
        }

        private void SetupPetProfile(bool petIsOwned)
        {
            _unavailablePet.SetActive(petIsOwned == false);
            _obtainPet.gameObject.SetActive(petIsOwned == false);
            _descriptionBackground.enabled = petIsOwned;
        }

        private void InitListeners(UnityAction onBackButton, UnityAction onObtainPet)
        {
            _backButton.onClick.RemoveAllListeners();
            _obtainPet.onClick.RemoveAllListeners();

            _obtainPet.onClick.AddListener(onObtainPet);
            
            _backButton.onClick.AddListener(() => gameObject.SetActive(false));
            
            _backButton.onClick.AddListener(onBackButton);
        }

        private void SetupPetDescription(PetAlbumData data, bool petIsOwned)
        {
            
            _description.gameObject.SetActive(petIsOwned);
            _description.SetTextFromLocalizedId(PetDescriptionLocalizations.Table, data.Id);
        }

        private void SetupPetProfile(PetAlbumData data, bool petIsOwned)
        {
            _petSpine.gameObject.SetActive(petIsOwned);
            _petImage.gameObject.SetActive(!petIsOwned);
            
            _petName.SetText(petIsOwned ? data.Name : "???");
            
            _petNameBackground.color = petIsOwned ? _petNameBackgroundColor : Color.white;
            
            _petStar.SetActive(petIsOwned);

            if (petIsOwned)
            {
                var spineData = MainSceneController.Instance.AssetLibrary.GetPetObject(data.Id).SkeletonDataAsset;
                _petSpine.SetPetAsset(spineData);
            }
            else
            {
                _petImage.sprite = data.GetImage(true);
                _petImage.color = _silhouetteColor;
            }
        }

        private void SetupBasicSkillDisplay(PetAlbumData data, bool petIsOwned)
        {
            _basicSkillInactiveBackground.enabled = petIsOwned == false;
            _basicSkillBackground.enabled = petIsOwned;
            if (petIsOwned)
            {
                _basicSkill.SetTextFromLocalizedId(PetSkillsLocalizations.Table, data.BasicSkill.Type, new Dictionary<string, string>()
                {
                    {"amount", data.BasicSkill.Amount.ToString()}
                });
            }
            else
            {
                _basicSkill.SetText("???");
            }
        }
    }
}
