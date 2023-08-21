using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Backend;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Data;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    public class RewardController
    {
        public UnityEvent<PlayerBalance> OnBalanceUpdate;

        #region INIT
        public void Init()
        {
            
        }
        #endregion

        #region LOBBY
        
        public void RemoveAllListener()
        {
            OnBalanceUpdate.RemoveAllListeners();
        }

        #endregion
    }
}