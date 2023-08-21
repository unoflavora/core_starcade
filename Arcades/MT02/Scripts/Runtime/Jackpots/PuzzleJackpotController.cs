using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots
{
    public class PuzzleJackpotController : MonoBehaviour
    {
        [SerializeField] private List<Image> puzzlePieces;
        [SerializeField] private Image _puzzleResult;
        [SerializeField] public UnityEvent onJackpot;

        [SerializeField] public List<int> currentJackpotState;
        [SerializeField] private List<Animator> _puzzleAnimator;
        
        public (Image, int) SetProgress(List<int> currentPuzzleProgress, bool enablePuzzle = true)
        {
            Image activatedPuzzleSprite = null;
            var activatedPuzzleIndex = 0;

            currentJackpotState = currentPuzzleProgress;
            for (int i = 0; i < puzzlePieces.Count; i++)
            {
                if (puzzlePieces[i].enabled == false && currentPuzzleProgress[i] > 0)
                {
                    activatedPuzzleSprite = puzzlePieces[i];
                    activatedPuzzleIndex = i;
                }
                
                if (enablePuzzle) puzzlePieces[i].enabled = currentPuzzleProgress[i] > 0;
            }

            CheckJackpot(puzzlePieces.Count);

            return (activatedPuzzleSprite, activatedPuzzleIndex);
        }

        
        private void CheckJackpot(int maxPuzzleCount)
        {
            for (int i = 0; i < maxPuzzleCount; i++)
            {
                if (currentJackpotState[i] < 1) return;
            }
            
            onJackpot.Invoke();
        }

        public void ActivateLight(bool activate)
        {
            foreach (var animator in _puzzleAnimator) animator.SetTrigger(activate ? "In" : "Out");
        }

        public async Task DisplayResultOverlay()
        {
            _puzzleResult.enabled = true;
            await Task.Delay(2000);
            _puzzleResult.enabled = false;
        }
    }
}
