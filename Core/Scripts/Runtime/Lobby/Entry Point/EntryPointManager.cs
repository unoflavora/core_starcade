using Agate.Starcade.Boot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.EntryPoint
{
    public class EntryPointManager : MonoBehaviour
    {
        [SerializeField] private Transform _entryPointContainer;
        [SerializeField] private GameObject _entryPointMenuItemPrefab;
        [SerializeField] private List<EntryPointMenuData> _entryPointMenuDatas;
        [SerializeField] private Transform _entryPointMenuItemContainer;
        [SerializeField] private bool _fullPreview;
        [SerializeField] private EntryPointMenuData _defaultEntryPointMenuData;
        [SerializeField] private Button _closeButton;


        private EntryPointMenuItem[] _entryPointMenuItems;
        private bool _isInitialize;

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            _entryPointContainer.gameObject.SetActive(true);

            if (!_isInitialize)
            {
                SetupEntryPoint();
            }
        }

        public void Close()
        {
            _entryPointContainer.gameObject.SetActive(false);
        }

        private void SetupEntryPoint()
        {
            _entryPointMenuItems = _entryPointMenuItemContainer.GetComponentsInChildren<EntryPointMenuItem>();

            for (int i = 0; i < _entryPointMenuItems.Length; i++)
            {
                if(i <= _entryPointMenuDatas.Count - 1)
                {
                    _entryPointMenuItems[i].Setup(_entryPointMenuDatas[i]);
                }
                else
                {
                    if (_fullPreview)
                    {
                        _entryPointMenuItems[i].Setup(_defaultEntryPointMenuData);
                    }
                    else
                    {
                        _entryPointMenuItems[i].gameObject.SetActive(false);
                    }
                }
            }

            _isInitialize = true;
        }
    }

    [Serializable]
    public class EntryPointMenuData
    {
        public string MenuName;
        public AssetReference SceneAsset;
        public Sprite MenuIcon;
    }
}