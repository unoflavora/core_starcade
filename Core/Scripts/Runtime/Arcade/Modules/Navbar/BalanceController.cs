using Starcade.Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.Navbar
{
    public class BalanceController : MonoBehaviour
    {
        [SerializeField] private NumberCounter balance;
        private double _balance;

        private void Awake()
        {
            balance.Duration = .25f;
        }

        public double Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                balance.Balance = _balance;
            }
        }
    }
}