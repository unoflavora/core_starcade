using Agate.Starcade.Runtime.Data;
using System;

namespace Agate.Starcade.Core.Runtime.Lobby.PetBox.Model
{
    [Serializable]
    public class PetBoxData
    {
        public string PetBoxId;
        public int PityAmount;
        public int PityIndex;
        public int TotalItem;
        public int RollCount;
        public RewardBase[] Rewards;
    }
}