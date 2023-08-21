using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Data;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.Navbar
{
    public class PlayerBalanceController : MonoBehaviour
    {
        [SerializeField] private BalanceController _goldCoin;
        [SerializeField] private BalanceController _starCoin;
        [SerializeField] private BalanceController _starTicket;

        public GameModeEnum GameMode;
        
        public double StarTicket
        {
            get => _starTicket.Balance;
            set => _starTicket.Balance = value;
        }

        public Vector3 BalancePos
        {
            get
            {
                switch (GameMode)
                {
                    case GameModeEnum.Gold: return _goldCoin.transform.position;
                    case GameModeEnum.Star: return _starCoin.transform.position;
                    default: return Vector3.zero;
                }
            }
        }

        public string Currency
        {
            get
            {
                switch (GameMode)
                {
                    case GameModeEnum.Gold: return "GoldCoin";
                    case GameModeEnum.Star: return "StarCoin";
                    default: return null;
                }
            }
        }
        public double Balance
        {
            get
            {
                switch (GameMode)
                {
                    case GameModeEnum.Star:
                        return _starCoin.Balance;
                    default:
                        return _goldCoin.Balance;
                }
            }
            set
            {
                switch (GameMode)
                {
                    case GameModeEnum.Star:
                        _starCoin.Balance = value;
                        break;
                    default:
                        _goldCoin.Balance = value;
                        break;
                }
            }
        }

        public void SetBalance(PlayerBalance balanceController)
        {
            _starTicket.Balance = balanceController.StarTicket;
            _goldCoin.Balance = balanceController.GoldCoin;
            _starCoin.Balance = balanceController.StarCoin;
        }
    }
}