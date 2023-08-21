using Agate.Starcade.Runtime.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Agate.SceneData;

namespace Agate.Starcade
{
    public class LoadingInfoController : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _backgroundImagePotrait;
        
        [SerializeField] private TMP_Text _infoTitle;
        [SerializeField] private TMP_Text _infoDesc;
        [SerializeField] private Image _infoImage;

        [SerializeField] private Transform _leftContainer;
        [SerializeField] private Transform _rightContainer;

        [SerializeField] private Transform _portraitContainer;
        [SerializeField] private Transform _landscapeContainer;

        [SerializeField] private Transform _potraitPanel;
        [SerializeField] private Transform _landscapePanel;

		[SerializeField] private GameObject _landscapeInfoFooter;

        [SerializeField] private GameObject _landscapeInfoDownloadFooter;
		[SerializeField] private TMP_Text _landscapeInfoDownloadText;

        [SerializeField] private Sprite _defaultBackground;
        
        public void SetUp(LoadingInfoData loadingInfoData, Sprite loadingBackground, SceneData.SceneOrientation sceneOrientation, bool isDownloading = false)
        {
			if(loadingBackground == null)
			{
				loadingBackground = _defaultBackground;
			}

			if(sceneOrientation == SceneOrientation.Portrait)
			{
				_backgroundImage.gameObject.SetActive(false);
				_backgroundImagePotrait.gameObject.SetActive(true);
				_backgroundImagePotrait.sprite = loadingBackground;
			}
			else
			{
				_backgroundImage.gameObject.SetActive(true);
				_backgroundImagePotrait.gameObject.SetActive(false);
				_backgroundImage.sprite = loadingBackground;
			}

            _infoTitle.text = loadingInfoData.Title;
            _infoDesc.text = loadingInfoData.InfoDesc;
            _infoImage.sprite = loadingInfoData.InfoSprite;

			switch (sceneOrientation)
			{
				case SceneData.SceneOrientation.Unknown:
					_potraitPanel.gameObject.SetActive(false);
					_landscapePanel.gameObject.SetActive(true);

					_rightContainer.SetParent(_landscapeContainer);
					_leftContainer.SetParent(_landscapeContainer);
					_leftContainer.SetAsFirstSibling();
					break;
				case SceneData.SceneOrientation.Portrait:
					_potraitPanel.gameObject.SetActive(true);
					_landscapePanel.gameObject.SetActive(false);

					_rightContainer.SetParent(_portraitContainer);
					_leftContainer.SetParent(_portraitContainer);
					_leftContainer.SetAsFirstSibling();
					break;
				case SceneData.SceneOrientation.Landscape:
					_potraitPanel.gameObject.SetActive(false);
					_landscapePanel.gameObject.SetActive(true);

					_rightContainer.SetParent(_landscapeContainer);
					_leftContainer.SetParent(_landscapeContainer);
					_leftContainer.SetAsFirstSibling();
					break;
				default:
					_potraitPanel.gameObject.SetActive(false);
					_landscapePanel.gameObject.SetActive(true);

					_rightContainer.SetParent(_landscapeContainer);
					_leftContainer.SetParent(_landscapeContainer);
					_leftContainer.SetAsFirstSibling();
					break;
			}

			if (isDownloading)
			{
                _landscapeInfoFooter.SetActive(false);
                _landscapeInfoDownloadFooter.SetActive(true);
			}
			else
			{
				_landscapeInfoFooter.SetActive(true);
                _landscapeInfoDownloadFooter.SetActive(false);
            }

		}

		public void UpdateDownloadIndicator(string text)
		{
			//string downloadText = "Downloading... " + percentage +"%";
			_landscapeInfoDownloadText.text = text;
		}
    }

    

    [Serializable]
    public class LoadingInfoData
    {
        public string Title;
        [TextArea(minLines: 2, maxLines: 4)] public string InfoDesc;
        public Sprite InfoSprite;
    }
}


