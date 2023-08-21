using Agate.Starcade.Runtime.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Runtime.Lobby.UserProfile
{
    public class EditProfileTabController : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private GameObject _background;
        [SerializeField] private TMP_Text _text;

        [SerializeField] private ItemTypeEnum _state;
        [SerializeField] private EditPhotoProfileController _editPhotoProfileController;

        void Start()
        {
            _toggle.onValueChanged.AddListener(value =>
            {
                _background.SetActive(value);
                Color color = value ? Color.white : new Color(156f/255f, 163f/255f, 175f/255f);
                _text.color = color;

                if (value)
                {
                    _editPhotoProfileController.ChangeTabState(_state);
                }
            });
        }
    }
}
