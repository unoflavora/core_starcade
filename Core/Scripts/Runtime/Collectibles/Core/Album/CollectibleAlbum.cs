using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using Agate.Starcade.Scripts.Runtime.Data_Class.Collectibles;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Album
{
    public class CollectibleAlbum : MonoBehaviour
    {
        [SerializeField] private PoolableGridUI _gridUI;
        [SerializeField] private ScrollRect _scrollUi;
        
        public Action<CollectibleSetData> OnAlbumClicked;
        
        /// <summary>
        ///  Set the data for the album
        /// </summary>
        /// <param name="albumCards"></param>
        public void SetAlbumData(List<CollectibleSetData> albumCards)
        {
            if(albumCards == null || albumCards.Count == 0)
                return;
            _gridUI.Draw(albumCards, obj => OnAlbumClicked.Invoke(obj.GetComponent<CollectibleAlbumCardUI>().Item));
        }

        private void OnEnable()
        {
            _scrollUi.verticalNormalizedPosition = 1;
        }
    }
}
