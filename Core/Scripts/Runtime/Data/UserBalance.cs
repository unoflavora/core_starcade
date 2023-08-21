using System;
using Agate.Starcade.Runtime.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Runtime.Data
{
	[Serializable]
    public class UserBalance
    {
		public decimal GoldCoin { get; set; }
		public decimal StarCoin { get; set; }
		public decimal StarTicket { get; set; }


		public decimal GetBalance(CurrencyTypeEnum currency)
		{
			return currency switch
			{
				CurrencyTypeEnum.GoldCoin => GoldCoin,
				CurrencyTypeEnum.StarCoin => StarCoin,
				CurrencyTypeEnum.StarTicket => StarTicket,
				_ => 0
			};
		}

		public decimal AddBalance(CurrencyTypeEnum currency, decimal amount)
		{
			return currency switch
			{
				CurrencyTypeEnum.GoldCoin => GoldCoin += amount,
				CurrencyTypeEnum.StarCoin => StarCoin += amount,
				CurrencyTypeEnum.StarTicket => StarTicket += amount,
				_ => 0
			};
		}

		public decimal ReduceBalance(CurrencyTypeEnum currency, decimal amount)
		{
			return currency switch
			{
				CurrencyTypeEnum.GoldCoin => GoldCoin -= amount,
				CurrencyTypeEnum.StarCoin => StarCoin -= amount,
				CurrencyTypeEnum.StarTicket => StarTicket -= amount,
				_ => 0
			};
		}
		
		public decimal SetBalance(CurrencyTypeEnum currency, decimal amount)
		{
			return currency switch
			{
				CurrencyTypeEnum.GoldCoin => GoldCoin = amount,
				CurrencyTypeEnum.StarCoin => StarCoin = amount,
				CurrencyTypeEnum.StarTicket => StarTicket = amount,
				_ => 0
			};
		}
	}
}
