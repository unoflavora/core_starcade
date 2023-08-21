using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.VFX
{
    public class PuzzleVFXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _puzzlePiece;

        public void SetMaterial(Material material)
        {
            _puzzlePiece.GetComponent<ParticleSystemRenderer>().material = material;
        }
    }
}
