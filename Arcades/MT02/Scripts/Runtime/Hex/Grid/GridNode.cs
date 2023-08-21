using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Modules.Hexa.Pathfinding;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.SO;
using TMPro;
using UnityEngine;

namespace Starcade.Arcades.MT02.Scripts.Runtime.Hex.Grid
{
    public class GridNode : MonoBehaviour, INode
    {
        [SerializeField] public MonstamatchSymbol symbol;
        [SerializeField] private PopVFXSO _popVfxso;
        [SerializeField] public ParticleSystem PopVfx;
        [SerializeField] private SpriteRenderer lineSprite;
        [SerializeField] private GameObject _highlight;

        public Coordinate coordinate {get; set;}
        public List<Coordinate> neighbours {get; set;}
        public List<Coordinate> upperNeighbours {get; set;}
        public List<Coordinate> lowerNeighbours {get; set;}
        public bool IsExplored;
        
        public TextMeshPro text;
        
        public GameObject selected;
        
        public void ShowSelectedVfx() 
        {
            selected.SetActive(true);
        }

        public async Task DestroySymbol(int objectCount)
        {
            var vfx = Instantiate(_popVfxso.GetVFX(objectCount), gameObject.transform);
            //PopVfx.gameObject.SetActive(true);
            vfx.Play();
            
            await Task.Delay(500); // await for the particle system to diverge first so to create impression of "pop"
            HideSelectedVfx();
            symbol.SetupData(null);
            
            // while (PopVfx.isPlaying) await Task.Delay(50);
            // PopVfx.gameObject.SetActive(false);
        }

        public void SetVFX(int objectCount)
        {
            PopVfx = _popVfxso.GetVFX(objectCount);
        }
        
        public void HideSelectedVfx()
        {
            selected.SetActive(false);
            ScaleDownSymbol();
        }

        public void EnablePointerTo(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            lineSprite.transform.rotation = lookRotation;
            lineSprite.enabled = true;
        }

        public void RemovePointer()
        {
            lineSprite.enabled = false;
        }

        public void EnableHighlight(bool enabled)
        {
            _highlight.SetActive(enabled);
        }

        public void ScaleUpSymbol()
        {
            symbol.ScaleUp();
        }

        public void ScaleDownSymbol()
        {
            symbol.ScaleDown();
        }
        
        

    };
}