using Agate.Starcade.Scripts.Runtime.DailyLogin;
using UnityEngine;
using UnityEngine.Serialization;

namespace Agate.Starcade.Runtime.SystemContainer
{
    public class SystemContainer : MonoBehaviour
    {
        public static SystemContainer Instance { get; private set; }

        [FormerlySerializedAs("_dailyLoginManager")] [SerializeField] private DailyLoginController dailyLoginController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
