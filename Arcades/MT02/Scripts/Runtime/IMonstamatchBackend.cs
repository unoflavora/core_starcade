using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Starcade.Arcades.MT02.Scripts.Runtime;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend
{
    public interface IMonstamatchBackend
    {
        public Task<MonstamatchJackpotData> UseJackpot(string sessionId);
        public Task<MonstamatchInitData> GetInitData(GameModeEnum mode);
        public Task<MonstamatchSpinData> Spin(string sessionId);
        public Task<MonstamatchMatchData> Match(string sessionId, List<Coordinate> paths);
        public Task<MonstamatchUseBulletData> UseBullet(string sessionId, int hitIndex);
        public Task BuyBullet(string sessionId, int index);
        public Task<MonstamatchUsePuzzleData> UsePuzzle(string sessionId);

        public Task<MonstamatchClaimRewardData> ClaimReward(string sessionId);
    }
}