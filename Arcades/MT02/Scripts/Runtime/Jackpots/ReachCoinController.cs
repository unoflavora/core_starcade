using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots
{
    public enum InstantJackpotType
    {
        None,
        Mini,
        Minor,
        Mayor // TODO if possible, Backend needs to change this into "Major"
    };
    
    public class ReachCoinController : MonoBehaviour
    {
        [SerializeField] private List<ReachCoin> coins;
        
        public UnityEvent<List<InstantJackpotType>> OnJackpot;
        private Dictionary<InstantJackpotType, bool> JackpotState;
        public bool CheckingJackpot;
        public List<InstantJackpotType> _currentSpinJackpot { get; set; }

        public void ResetState()
        {
            JackpotState = new Dictionary<InstantJackpotType, bool>();
            JackpotState[InstantJackpotType.Mini] = false;
            JackpotState[InstantJackpotType.Minor] = false;
            JackpotState[InstantJackpotType.Mayor] = false;
            
            coins.ForEach(coin => coin.EnableCoin(false));
        }
        
     
        public List<ReachCoin> AddCoin(int count, bool activateIcon = false)
        {
            if (count == 0) return null;
            
            _currentSpinJackpot = new List<InstantJackpotType>();
            List<ReachCoin> addedCoin = new();
            int added = 0;
            
            foreach (var coin in coins)
            {
                if (!coin.IsActive)
                {
                    added++;
                    coin.EnableCoin(activateIcon);
                    addedCoin.Add(coin);
                    coin.IsActive = true;
                    if (added == count) break;
                }
            }
            
            CheckJackpot();

            CheckingJackpot = false;
            return addedCoin;
        }

        private void CheckJackpot()
        {
            var activeCoin = 0;
            foreach (var coin in coins)
            {
                if (coin.IsActive)
                {
                    activeCoin++;
                }
            }

            if (activeCoin >= 5) ProcessJackpot(InstantJackpotType.Mini);
            
            if (activeCoin >= 9) ProcessJackpot(InstantJackpotType.Minor);
            
            if (activeCoin == 12) ProcessJackpot(InstantJackpotType.Mayor);

            if (_currentSpinJackpot.Count > 0)
            {
                OnJackpot.Invoke(_currentSpinJackpot);
            }
            // return coins.TrueForAll(coin => coin.IsActive);
        }

        private void ProcessJackpot(InstantJackpotType type)
        {
            if (JackpotState[type]) return;
            
            JackpotState[type] = true;
            _currentSpinJackpot.Add(type);
        }

        private static bool IsBetween(int x, int lower, int upper) {
            return lower <= x && x <= upper;
        }
        
    }
}