using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.SO
{
    [CreateAssetMenu(menuName = "Starcade/Arcade/Monstamatch/PopVFXData")]
    public class PopVFXSO : ScriptableObject
    {
        [SerializeField] private List<ParticleSystem> _vfxList;

        public ParticleSystem GetVFX(int objectCount)
        {
            var vfx = objectCount switch
            {
                < 6 => _vfxList[0],
                < 10 => _vfxList[1],
                < 15 => _vfxList[2],
                _ => _vfxList[3]
            };
            return vfx;
        }
    }
}
