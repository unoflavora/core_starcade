using System.Collections.Generic;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Grid
{
    public class PaytableConfig
    {
        public List<List<int>> BoardPins { get; set; }
        public int MaxSymbolPerMatch { get; set; }
        public int MaxMatchPerSpin { get; set; }
    }
}