using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Config;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots
{
    public class JackpotController : MonoBehaviour
    {
        [SerializeField] private ReachCoinController reachCoinController;
        [SerializeField] private ProgressBarController progressBarController;
        [SerializeField] private PuzzleJackpotController puzzleJackpotController;

        [NonSerialized] public UnityEvent<MonstamatchJackpotEnums> OnJackpot;
        public List<InstantJackpotType> CurrentInstantJackpot { get; set; }
        
        public void Init(JackpotConfig jackpotConfig, MonstamatchGameData data)
        {
            OnJackpot = new UnityEvent<MonstamatchJackpotEnums>();
            progressBarController.Init(jackpotConfig.TotalProgress, data.JackpotProgress);
            puzzleJackpotController.SetProgress(data.CurrentPuzzleJackpotProgress);
            ResetCoinJackpot();
            AddJackpotListener();
        }

        public async Task AddProgress(float progress)
        {
            await progressBarController.AddProgress(progress);
        }

        public List<ReachCoin> AddCoin(int count, bool activateIcon)
        {
            return reachCoinController.AddCoin(count, activateIcon);
        }

        
        public void ResetCoinJackpot()
        {
            CurrentInstantJackpot = new List<InstantJackpotType>();
            reachCoinController.ResetState();
        }

        public (Image, int) ConfigPuzzle(List<int> count, bool enablePuzzle = true)
        {
            return puzzleJackpotController.SetProgress(count, enablePuzzle);
        }

        public async Task StartPuzzleSeq()
        {
            puzzleJackpotController.ActivateLight(true);
            await Task.Delay(2000);
        }

        public async Task StopPuzzleSeq()
        {
            await puzzleJackpotController.DisplayResultOverlay();
            puzzleJackpotController.ActivateLight(false);
        }

        private void AddJackpotListener()
        {
            OnJackpot = new UnityEvent<MonstamatchJackpotEnums>();
            
            puzzleJackpotController.onJackpot.AddListener(delegate
            {
                OnJackpot.Invoke(MonstamatchJackpotEnums.PuzzleJackpot);
            });
            
            progressBarController.onJackpot.AddListener(delegate
            {
                OnJackpot.Invoke(MonstamatchJackpotEnums.Jackpot);
            });
            
            reachCoinController.OnJackpot.AddListener(type =>
            {
                CurrentInstantJackpot = type;
                OnJackpot.Invoke(MonstamatchJackpotEnums.Coin);
            });
        }
    }
}