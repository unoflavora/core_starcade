using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.VFX
{
    public class PuzzleVFXManager : MonoBehaviour
    {
        [SerializeField] private List<Material> _vfxMaterialList;
        [SerializeField] private PuzzleVFXController _vfxPrefab;
        
        public void PlayVFX(int index)
        {
            _vfxPrefab.SetMaterial(_vfxMaterialList[index]);
            
            Instantiate(_vfxPrefab,gameObject.transform);
        }
    }
}
