using System.Threading.Tasks;
using UnityEngine;

namespace Agate
{
    public class Blinking : MonoBehaviour
    {
        [Tooltip("In Millisecond")] public int Delay;
        public bool IsBlinking = true;
        
        private void Start()
        {
            Blink();
        }

        public async Task Blink()
        {
            while (this.enabled)
            {
                if (!IsBlinking)
                {
                    gameObject.SetActive(false);
                    return;
                }
                gameObject.SetActive(true);
                await Task.Delay(Delay);
                gameObject.SetActive(false);
                await Task.Delay(Delay);
            }
        }
    }
}
