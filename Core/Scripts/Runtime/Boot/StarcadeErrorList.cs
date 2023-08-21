using Agate.Starcade.Boot;
using UnityEngine;

namespace Agate.Starcade
{
    [CreateAssetMenu(fileName = "ErrorList", menuName = "Starcade/StarcadeErrorList", order = 1)]
    public class StarcadeErrorList : ScriptableObject
    {
        public StarcadeError[] ListStarcadeError;
    }
}
