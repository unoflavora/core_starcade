using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate
{
    public class ArcadeDownloadController : MonoBehaviour
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _description;

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void Show(float size, UnityAction OnYes)
        {
            _yesButton.onClick.RemoveAllListeners();
            _yesButton.onClick.AddListener(() =>
            {
                OnYes();
                SetVisible(false);
            });
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(() => SetVisible(false));
            _description.text = $"This asset bundle size is {size} MB, are you sure want to download it?";
            SetVisible(true);
        }
    }
}
