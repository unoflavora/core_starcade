namespace Agate.Starcade.Scripts.Runtime.Data
{
    public class CollectCoinData
    {
        public double GoldCoinCollected { get; set; }
        public string LastCollectCoinTime { get; set; }
        public string NextCollectCoinTime { get; set; }
        public PlayerBalance Balance { get; set; }
    }
}