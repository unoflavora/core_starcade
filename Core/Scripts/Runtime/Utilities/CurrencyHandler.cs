using System;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public abstract class CurrencyHandler
    {
        public static string Convert(int currency, int minPower = 8)
        {
            double convertedCurrency = (double)currency;
            string convertedText = Convert(convertedCurrency, minPower);
            return convertedText;

        }

        public static string Convert(float currency, int minPower = 8)
        {
            double convertedCurrency = (double)currency * 10;
            string convertedText = Convert(convertedCurrency / 10, minPower);
            return convertedText;
        }

        public static string Convert(double currency, int minPower = 8)
        {
            string convertedText = "";
            double convertedCurrency = currency;
            if (currency < Mathf.Pow(10, minPower))
            {
                convertedText = Format(convertedCurrency, 0, 0);
                return convertedText;
            }

            if (currency >= Mathf.Pow(10, 12))
            {
                convertedText = Rearrange(convertedCurrency, 12, 0);
            }
            else if (currency >= Mathf.Pow(10, 9))
            {
                convertedText = Rearrange(convertedCurrency, 9, 0);
            }
            else if (currency >= Mathf.Pow(10, 6))
            {
                convertedText = Rearrange(convertedCurrency, 6, 0);
            }
            return convertedText;
        }

        private static string Rearrange(double number, int tenPower, int decimalIndex)
        {
            double convertedNumber = number / Mathf.Pow(10, (tenPower));
            string convertedText = Format(convertedNumber, tenPower, decimalIndex);
            return convertedText;
        }

        public static string Format(double currency, int tenPower = 0, int decimalIndex = 0)
        {
            string[] units = new string[5] { "", "K", "M", "B", "T" };
            string[] textNumber = $"{currency}".Split('.');
            string trimmedCurrency = textNumber[0];
            string marginCurrency = decimalIndex > 0 ? $".{GetDecimalNumber(textNumber[1] == null ? "" : textNumber[1], decimalIndex)}" : "";

            string formattedCurrency = "";
            int length = (trimmedCurrency.Length / 3) + 1;
            int first = 0;
            int end = trimmedCurrency.Length % 3;
            if (end <= 0)
            {
                length -= 1;
                end = 3;
            }
            for (int i = 0; i < length; i++)
            {
                if (i <= 0)
                {
                    formattedCurrency = $"{trimmedCurrency.Substring(first, end)}";
                }
                else
                {
                    formattedCurrency = $"{formattedCurrency},{trimmedCurrency.Substring(first, end)}";
                }
                first += end;
                end = 3;
            }
            formattedCurrency = $"{formattedCurrency}{marginCurrency}{units[tenPower / 3]}";
            return formattedCurrency;
        }
        
        
        // Issues in Format where currency is maxed at 3 decimal value
        // Use this instead
        public static string FormatNumber(double number)
        {
            const double Billion = 1_000_000_000;
            const double Million = 1_000_000;
            const double Thousand = 1_000;

            if (number >= Billion)
                return $"{number / Billion:#,##0.##}B";
            if (number >= Million)
                return $"{number / Million:#,##0.##}M";
            if (number >= Thousand)
                return $"{number / Thousand:#,##0.##}K";
            return number.ToString("#,##0");
        }

        private static string GetDecimalNumber(string currency, int decimalIndex)
        {
            string marginCurrency = currency;
            marginCurrency = decimalIndex > marginCurrency.Length ? $"{marginCurrency}{new String('0', decimalIndex)}" : marginCurrency;
            marginCurrency = marginCurrency.Substring(0, decimalIndex);
            return marginCurrency;
        }
    }
}
