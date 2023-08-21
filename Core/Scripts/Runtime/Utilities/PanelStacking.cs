using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Agate
{
    public static class PanelStacking
    {

        private static bool isDone;

        public static async Task StartWait()
        {
            isDone = false;
            await WaitDone();
        }

        public static void StopWait()
        {
            isDone = true;
        }
        
        public static async Task WaitDone()
        {
            while (!isDone)
            {
                await Task.Yield();
            }
        }
    }
}
