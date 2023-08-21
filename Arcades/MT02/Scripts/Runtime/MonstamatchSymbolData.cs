using System;
using Agate.Modules.Hexa.Pathfinding;
using UnityEngine;

namespace Starcade.Arcades.MT02.Scripts.Runtime
{
    [Serializable]
    public class MonstamatchSymbolData : ISymbol
    {
        public string _id;
        public int _index;
        public Sprite sprite;
        public MonstaMatchSymbolTypesEnum _type;
        public double _percentage;
        public int JackpotValue { get; set; }
        public bool IsSpecial { get; set; }
        public string Id { get => _id; set => _id = value; }
        public int Index { get => _index; set => _index = value; }
        public MonstaMatchSymbolTypesEnum Type { get => _type; set => _type = value; }
        public double Percentage { get => _percentage; set => _percentage = value; }

        public MonstamatchSymbolData(MonstamatchSymbolData symbol)
        {
            IsSpecial = false;

            if (symbol != null)
            {
                Id = symbol.Id;
                Index = symbol.Index;
                Percentage = symbol.Percentage;
                Type = symbol.Type;
                JackpotValue = symbol.JackpotValue;
                sprite = symbol.sprite;
            }
        }

        public MonstamatchSymbolData(string id, int index, double percentage,
            MonstaMatchSymbolTypesEnum type, int jackpotValue)
        {
            IsSpecial = false;

            Id = id;
            Index = index;
            Percentage = percentage;
            Type = type;
            JackpotValue = jackpotValue;
        }
        
        
        public MonstamatchSymbolData()
        {
          
        }
    }
}
