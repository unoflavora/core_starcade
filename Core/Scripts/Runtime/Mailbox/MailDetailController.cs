using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class MailDetailController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private Image _banner;
        [SerializeField] private TextMeshProUGUI _content;
        [SerializeField] private Button _closeButton;

        [SerializeField] private LinkOpener _linkOpener;
        [SerializeField] private Sprite _defaultIcon;

        [SerializeField] private ContentSizeFitter _contentSizeFitter;

        [SerializeField] private CanvasGroup _contentCanvasGroup;

        private MailboxDataItem _data;

        [HideInInspector] public UnityEvent OnCloseEvent;

        public void Init()
        {
            _closeButton.onClick.AddListener(() => 
            {
                OnCloseEvent.Invoke();
            });
        }

        public void OnDisable()
        {
            Debug.Log("CLOSE");
        }

        public async void ResetContent(MailboxDataItem data)
        {
            Debug.Log("SET CONTENT");
            _contentSizeFitter.enabled = false;
            _contentCanvasGroup.alpha = 0;
            _data = data;
            Sprite sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data.Header.Icon);
            _icon.sprite = sprite == null ? _defaultIcon : sprite;
            _title.text = $"{data.Header.Tittle}";
            DateTime recievedAt = DateTime.Parse(data.StatusData.ReceivedAt).ToUniversalTime();
            string daySuffix = GetDaySuffix(recievedAt.Day);
            _status.text = $"{data.Header.Description}\n{recievedAt.ToString("ddd, MMMM dd")}{daySuffix} {recievedAt.ToString("yyyy")}";
            if (!string.IsNullOrEmpty(data.Content.BannerId))
            {
                _banner.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data.Content.BannerId);
                _banner.gameObject.SetActive(true);
            }
            else
            {
                _banner.gameObject.SetActive(false);
            }
            string link = '"' + $"{data.Content.CustomLink}" + '"';
            _content.text = $"{data.Content.Text}\n\n<b><u><link={link}>{data.Content.CustomLinkCaption}</link></u></b>";

            LeanTween.value(0, 1, 0.5f).setEaseInExpo().setOnUpdate((float opacity) =>
            {
                _contentCanvasGroup.alpha = opacity;
            });

            await Task.Delay(100);
            _contentSizeFitter.enabled = true;
            Debug.Log("SET CONTENTT");
            //_contentSizeFitter.SetLayoutHorizontal();
            //_contentSizeFitter.SetLayoutVertical();
        }

        public void ResetLayout()
        {
            Debug.Log("reset");
        }

        public void RemoveAllListeners()
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        private string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}
