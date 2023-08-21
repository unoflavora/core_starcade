using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data
{
    public class MonstamatchUseBulletData
    {
        public MonstamatchGameData MonstamatchGameData { get; set; }
        public SessionData SessionData { get; set; }
        public PlayerBalance PlayerBalance { get; set; }
        public int RewardIndex { get; set; }
    }
}