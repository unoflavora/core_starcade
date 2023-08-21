using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.Core
{
    public class PetLevelUpCalculator : MonoBehaviour
    {
        public static class PetLevelUpExpCalculator
        {
            public static int GetTotalExpAtLevel(int level, PetLevelUpFormula f)
            {
                var result = level / 2 * (2 * 2 + (level - 1) * f.X);
                if (level > 10)
                    result += (level - 10) / 2 * (level - 9) * (f.Y - f.X);
                return result;        
            } 
        
        }
    }
}
