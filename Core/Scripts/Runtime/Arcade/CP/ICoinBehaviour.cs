using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Arcade.CoinPusher
{
    public interface ICoinBehaviour
    {
        public UnityEvent OnGoalEvent { get; set; }
        public bool IsActive { get; set; }
        public virtual void OnAwake() { }
        public virtual void OnPlay() { }
        public virtual void OnPause() { }
        public virtual void OnGoal(int type) { }
        public virtual void OnFail(int type) { }
    }
}