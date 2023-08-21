using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Collectibles.UI;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Album
{
    public class CollectibleAlbumCardUI : MonoBehaviour, IPoolableGridItem<CollectibleSetData>
    {
        [Header("Album Object")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _completedText;
        [SerializeField] private TextMeshProUGUI _textCount;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Image _albumImage;
        
        [Header("Data")]
        [SerializeField] private Sprite _defaultImage;

        private Button _clickable;
        public CollectibleSetData Item { get; private set; }
        public Action<CollectibleSetData> OnItemClicked { get; set; }
        
        private void Start()
        {
            _clickable = GetComponent<Button>();
            _clickable.onClick.AddListener(delegate { OnItemClicked(Item); });
        }

        public void SetupData(CollectibleSetData setData, int index, GridUIOptions uiOptions = null)
        {
            Item = setData;
            
            if (Item == null) throw new Exception("Instantiated Object is not Collectible Album Card");
            
            SetTitle(Item.CollectibleSetName);
            
            if (setData.IsComingSoon)
            {
                _albumImage.gameObject.SetActive(false);
                SetProgress(0,15);
                return;
            }

            _albumImage.gameObject.SetActive(true);
            
            var currentOwnedItemCount = setData.CollectibleItems.FindAll(item => item.Amount > 0).Count;
            
            SetProgress(currentOwnedItemCount,Item.CollectibleItems.Count);
            
            SetAlbumCover(setData.CollectibleSetId);
        }


        private void SetAlbumCover(string imageId)
        {
            Sprite image = MainSceneController.Instance.AssetLibrary.GetSpriteAsset(imageId);
            
            _albumImage.sprite = image == null ? _defaultImage : image;
        }

        private void SetTitle(string title)
        {
            _title.SetText($"{title}");
        }
        
        private void SetProgress(int currentCount, int maxCount)
        {
            var isComplete = currentCount == maxCount;
            
            if (!isComplete)
            {
                _textCount.SetText($"{currentCount} / {maxCount}");
                _progressBar.maxValue = maxCount;
                _progressBar.value = currentCount;
            }
            
            _completedText.gameObject.SetActive(isComplete);
            _textCount.gameObject.SetActive(!isComplete);
            _progressBar.gameObject.SetActive(!isComplete);
        }
    }
}
