using System.Threading.Tasks;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.MiniGames
{
    public class RewardAnimationController : MonoBehaviour
    {
        public void FinishOpenAnimation()
        {
            gameObject.SetActive(false);
        }
        
        public Task SetRouletteRewardVFXState(bool isActive)
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Animator>().SetTrigger(isActive ? "In" : "Out");
            return Task.CompletedTask;
        }
    }
}
