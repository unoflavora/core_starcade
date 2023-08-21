using System;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.UI;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.Album.UI
{
    public class PetAlbumItem : MonoBehaviour, IPoolableGridItem<PetAlbumData>, IPointerClickHandler
    {
        [Header("UI Objects")]
        [SerializeField] private TextMeshProUGUI _itemIndex;
        [SerializeField] private TextMeshProUGUI _petName;
        [SerializeField] private Image _petImage;
        [SerializeField] private Image _albumBackground;

        [Header("Data")]
        [SerializeField] private Sprite _activeBackground;
        [SerializeField] private Sprite _inactiveBackground;
        [SerializeField] private Color _silhouetteColor;
        
        public Action<PetAlbumData> OnItemClicked { get; set; }

        private PetAlbumData _data;
        public PetAlbumData Data => _data;
        
        public void SetupData(PetAlbumData data, int index, GridUIOptions uiOptions = null)
        {
            _data = data;
            
            var isPetOwned = data.HasOwned;

            _petImage.sprite = data.GetImage(data.HasOwned == false);
            
            _petImage.color =  isPetOwned ? Color.white : _silhouetteColor;

            _albumBackground.sprite = isPetOwned ? _activeBackground : _inactiveBackground;
            
            _itemIndex.SetText("No. " + (index + 1));

            _petName.SetText(isPetOwned ? data.Name : "???");

            transform.name = data.Name;
            
            GetComponent<CanvasTransition>().TriggerTransition();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnItemClicked.Invoke(_data);
        }
    }
}
