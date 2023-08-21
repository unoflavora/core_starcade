using Agate.Starcade.Runtime.Helper;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Info;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby.EntryPoint
{
    public class EntryPointMenuItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleMenu;
        [SerializeField] private Image _iconMenu;

        private Button _menuButton;
        private string _scenePath;
        private AssetReference _sceneAssetReference;

        public void Setup(EntryPointMenuData menuData)
        {
            _menuButton = this.gameObject.GetComponent<Button>();

            _titleMenu.text = menuData.MenuName;
            _iconMenu.sprite = menuData.MenuIcon;
            _sceneAssetReference = menuData.SceneAsset;

            _menuButton.onClick.RemoveAllListeners();
            _menuButton.onClick.AddListener(OpenMenu);
        }

        public void OpenMenu()
        {
            if(_sceneAssetReference.AssetGUID == string.Empty)
            {
                MainSceneController.Instance.Info.Show(string.Empty, "Coming Soon!", Starcade.Runtime.Enums.InfoIconTypeEnum.Alert, new InfoAction("Back", null), null);
            }
            else
            {
                LoadSceneHelper.LoadSceneAdditive(_sceneAssetReference);
            }
        }
    }
}