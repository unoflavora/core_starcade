using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Data;
using Agate.Starcade.Runtime.Enums;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade
{
    public class CollectPopupController : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;

        [Header("Single")]
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;

        [Header("Multiple")]
        [SerializeField] private Transform _parent;
        [SerializeField] private GameObject _itemPrefab;

        private List<GameObject> _itemList;

        public UnityEvent OnCloseEvent;

        public void Init()
        {
            OnCloseEvent.RemoveAllListeners();

            _itemList = new List<GameObject>();
            _closeButton.onClick.AddListener(() =>
            {
                SetVisible(false);
                OnCloseEvent.Invoke();
            });
        }

        public void SetContent(RewardBase data)
        {
            _icon.sprite = ShopItemAssetHelper.GetAsset(data);
            int currencyId = -1;
            if (data.Type == RewardEnum.GoldCoin)
            {
                currencyId = 0;
            }
            else if (data.Type == RewardEnum.StarCoin)
            {
                currencyId = 1;
            }
            else if (data.Type == RewardEnum.StarTicket)
            {
                currencyId = 2;
            }
            string icon = currencyId > -1 ? $"<sprite={currencyId}> " : "";
            _amount.text = $"{icon}{data.Amount}";
        }

        public void SetContent(List<RewardBase> data)
        {
            Debug.Log("DATA SET CONTENT = " + JsonConvert.SerializeObject(data));

            if (_itemList.Count > data.Count)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    RewardIconObject mailReward = _itemList[i].GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], false);
                    _itemList[i].SetActive(true);
                }
                for (int i = data.Count; i < _itemList.Count; i++)
                {
                    _itemList[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _itemList.Count; i++)
                {
                    Debug.Log("lewat sini dulu");
                    RewardIconObject mailReward = _itemList[i].GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], false);
                    _itemList[i].SetActive(true);
                }
                for (int i = _itemList.Count; i < data.Count; i++)
                {
                    Debug.Log("lewat sini dulu lagi");
                    GameObject gameObject = Instantiate(_itemPrefab, _parent);
                    RewardIconObject mailReward = gameObject.GetComponent<RewardIconObject>();
                    mailReward.SetContent(data[i], false);
                    gameObject.SetActive(true);
                    _itemList.Add(gameObject);
                }
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
