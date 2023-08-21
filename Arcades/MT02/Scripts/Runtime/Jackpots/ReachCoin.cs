using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots
{
    public class ReachCoin : MonoBehaviour
    {
        [SerializeField] public Image CoinImage;
        public bool IsActive = false;
        
        public void EnableCoin(bool isEnabled)
        {
            IsActive = isEnabled;
            CoinImage.enabled = isEnabled;
        }
        
    }
}
