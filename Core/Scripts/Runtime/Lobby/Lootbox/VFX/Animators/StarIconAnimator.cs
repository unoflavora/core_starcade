using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Scripts.Runtime.Enums.Lootbox;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Core.Runtime.Lobby.Lootbox.Animators
{
    public class StarIconAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private List<Texture> _textures;

        public UnityEvent OnFinishStar = new UnityEvent();

        bool _isAnimFinished;

        // Event Function
        public void AnimationFinished()
        {
            _isAnimFinished = true;
        }

        public async Task Open(LootboxRarityEnum rarityEnum)
        {
            Texture texture;
            switch (rarityEnum)
            {
                case LootboxRarityEnum.Bronze:
                    texture = _textures[0];
                    break;
                case LootboxRarityEnum.Silver:
                    texture = _textures[1];
                    break;
                case LootboxRarityEnum.Gold:
                    texture = _textures[2];
                    break;
                default:
                    texture = _textures[0];
                    break;
            }
            
            GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", texture);
            
            await WaitForAnimation();
        }
        
        public async Task Close()
        {
            _animator.SetTrigger("Close");
            
            await WaitForAnimation();
        }

        public void ForceClose()
        {
            this.gameObject.SetActive(false);
        }

        private async Task WaitForAnimation()
        {
            await AsyncUtility.WaitUntil(() => _isAnimFinished);

            _isAnimFinished = false;
        }

       
        public void Finish()
        {
            OnFinishStar.Invoke();
        }

    }
}