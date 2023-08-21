using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Game
{
    public class MailItemController : MonoBehaviour
    {

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private Button _claimButton;
        [SerializeField] private Button _readmoreButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private TextMeshProUGUI _duration;
        [SerializeField] private Image _notification;

        [SerializeField] private Sprite _defaultIcon;

        private MailboxMenuEnum _tabState = MailboxMenuEnum.Collect;
        private string _id;
        private Button _currentButton;

        public void Init(MailboxMenuEnum tabState)
        {
            RemoveAllListeners();
            SetState(tabState);
        }

        public void SetState(MailboxMenuEnum tabState)
        {
            SetButtonVisible(_tabState, false);
            SetButtonVisible(tabState, true);
            _tabState = tabState;
        }

        public void SetButtonVisible(MailboxMenuEnum tabState, bool visible)
        {
            if (!visible && _currentButton != null) _currentButton.onClick.RemoveAllListeners();
            if (tabState == MailboxMenuEnum.Collect)
            {
                _claimButton.gameObject.SetActive(visible);
                _currentButton = _claimButton;
            }
            else if (tabState == MailboxMenuEnum.Community)
            {
                _joinButton.gameObject.SetActive(visible);
                _currentButton = _joinButton;
            }
            else if (tabState == MailboxMenuEnum.Information || tabState == MailboxMenuEnum.System)
            {
                _readmoreButton.gameObject.SetActive(visible);
                _currentButton = _readmoreButton;
            }
        }

        public void SetContent(string id, MailDataHeader data, MailDataStatus status)
        {
            MailboxMenuEnum state = (MailboxMenuEnum)Enum.Parse(typeof(MailboxMenuEnum), data.Category);
            SetState(state);

            _id = id;
            Sprite sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data.Icon);
            _icon.sprite = sprite == null && (state == MailboxMenuEnum.System || state == MailboxMenuEnum.Information) ? _defaultIcon : sprite;
            _title.text = data.Tittle;
            _desc.text = data.Description;
            if (state == MailboxMenuEnum.System || state == MailboxMenuEnum.Information)
            {
                TimeSpan expiredSpan = DateTime.Parse(status.ExpiredAt).ToUniversalTime() - DateTime.UtcNow;
                string text = expiredSpan.Days > 0 ? $"{expiredSpan.Days} Days Left" : $"{expiredSpan.Hours} Hours Left";
                _duration.text = text;
            }

            //if (DateTime.Parse(status.ReadAt).ToUniversalTime() > DateTime.MinValue)
            //    _notification.gameObject.SetActive(false);
            //else
            //    _notification.gameObject.SetActive(true); // true
        }

        public void SetOnclickListener(UnityAction<string> OnClick)
        {
            if (_currentButton != null) _currentButton.onClick.AddListener(() => OnClick(_id));
        }

        public void RemoveAllListeners()
        {
            _claimButton.onClick.RemoveAllListeners();
            _readmoreButton.onClick.RemoveAllListeners();
            _joinButton.onClick.RemoveAllListeners();
        }
    }
}
