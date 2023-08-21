using System;
using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Main;

namespace Agate.Starcade.Scripts.Runtime.Utilities
{
    public static class AsyncUtility
    {
        static int timer = 0;

        public static async Task WaitUntil(Func<bool> predicate)
        {
            while (!predicate()) await Task.Delay(50);
        }
        
        /// <summary>
        /// Start loading sequence from main scene launcher if predicate is not yet met after waitTime
        /// </summary>
        /// <param name="predicate">function that return true/false</param>
        /// <param name="waitTime"></param>
        ///
        //TODO : make this multi threadingsafe if called multiple times
        public static async void StartLoadingSeq(Func<bool> predicate, int waitTime = 2000)
        {
            while (!predicate())
            {
                timer += 100;
                
                if (timer >= waitTime)
                {
                    MainSceneController.Instance.Loading.StartLoading(LoadingScreen.LOADING_TYPE.MiniLoading);
                }
                
                await Task.Delay(100);
                
                Task.Yield();
            }

            timer = 0;
            
            MainSceneController.Instance.Loading.DoneLoading();
        }
    }
}