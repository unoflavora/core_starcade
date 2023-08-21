using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid;
using TMPro;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Album.UI
{
    public class PetAlbumView : MonoBehaviour
    {
        [SerializeField] private PoolableGridUI _albumGrid;
        
        [SerializeField] private TextMeshProUGUI _petCollectedText;
        
        public void DisplayPets(List<PetAlbumData> petData, int userPetCount, Action<PetAlbumData> onPetItemClick)
        {
            _petCollectedText.SetText("Pets Collected " + userPetCount + "/" + petData.Count);
            
            _albumGrid.Draw(petData, pet => onPetItemClick.Invoke(pet.GetComponent<PetAlbumItem>().Data), playAudioOnClick: false);
        }
    }
}
