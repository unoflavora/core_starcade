using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
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
using static Agate.Starcade.Runtime.Main.MainSceneController.AUDIO_KEY;

namespace Agate.Starcade
{
    public class MailCollectController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _status;

        [SerializeField] private Button _collectButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _itemPrefab;

        private AudioController _audio;
        private MailboxDataItem _data;
        private List<GameObject> _itemList;

        [HideInInspector] public UnityEvent<string> OnCollectEvent;
        [HideInInspector] public UnityEvent OnCloseEvent;


        public void Init()
        {
            _audio = MainSceneController.Instance.Audio;
            _itemList = new List<GameObject>();
            _collectButton.onClick.AddListener(() => 
            {
                _audio.PlaySfx(MAILBOX_CLAIM);
                OnCollectEvent.Invoke(_data.MailboxId);
            });

            _closeButton.onClick.AddListener(() =>
            {
                OnCloseEvent.Invoke();
            });
        }

        public void ResetContent(MailboxDataItem data)
        {
            _data = data;
            _icon.sprite = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(data.Header.Icon);
            _title.text = $"{data.Header.Tittle}";
            DateTime recievedAt = DateTime.Parse(data.StatusData.ReceivedAt).ToUniversalTime();
            string daySuffix = GetDaySuffix(recievedAt.Day);
            _status.text = $"{data.Header.Description}\n{recievedAt.ToString("ddd, MMMM dd")}{daySuffix} {recievedAt.ToString("yyyy")}";
            SetItemList(data.Rewards);
        }

        public void RemoveAllListeners()
        {
            _collectButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        private void SetItemList(RewardBase[] data)
        {
            if (_itemList.Count > data.Length)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    RewardIconObject mailReward = _itemList[i].GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], true);
                    _itemList[i].SetActive(true);
                }
                for (int i = data.Length; i < _itemList.Count; i++)
                {
                    _itemList[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _itemList.Count; i++)
                {
                    RewardIconObject mailReward = _itemList[i].GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], true);
                    _itemList[i].SetActive(true);
                }
                for (int i = _itemList.Count; i < data.Length; i++)
                {
                    GameObject gameObject = Instantiate(_itemPrefab, _container);
                    RewardIconObject mailReward = gameObject.GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], true);
                    gameObject.SetActive(true);
                    _itemList.Add(gameObject);
                }
            }
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
