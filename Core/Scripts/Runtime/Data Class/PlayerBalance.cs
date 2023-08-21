using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Runtime.Main;
using System;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class PlayerBalance
    {
        public double GoldCoin { get; set; }
        public double StarCoin { get; set; }
        public double StarTicket { get; set; }
    }
 
    public class PlayerBalanceActions
    {
        public Action<PlayerBalance> OnBalanceChanged { get; set; }

        public void AddBalance(CurrencyTypeEnum currency, float cost)
        {
            PlayerBalance playerBalance = MainSceneController.Instance.Data.UserBalance;

            switch (currency)
            {
                case CurrencyTypeEnum.GoldCoin:
                    playerBalance.GoldCoin += cost;
                    break;
                case CurrencyTypeEnum.StarCoin:
                    playerBalance.StarCoin += cost;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    playerBalance.StarTicket += cost;
                    break;
            }

            OnBalanceChanged?.Invoke(playerBalance);
        }

        public void ReduceBalance(CurrencyTypeEnum currency, float cost)
        {
            PlayerBalance playerBalance = MainSceneController.Instance.Data.UserBalance;

            switch (currency)
            {
                case CurrencyTypeEnum.GoldCoin:
                    playerBalance.GoldCoin -= cost;
                    break;
                case CurrencyTypeEnum.StarCoin:
                    playerBalance.StarCoin -= cost;
                    break;
                case CurrencyTypeEnum.StarTicket:
                    playerBalance.StarTicket -= cost;
                    break;
            }

            OnBalanceChanged?.Invoke(playerBalance);
        }
    }
}