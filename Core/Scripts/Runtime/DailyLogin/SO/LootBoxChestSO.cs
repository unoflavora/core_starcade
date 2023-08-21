using System;
using Agate.Starcade.Boot;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.DailyLogin.SO
{
    [CreateAssetMenu(menuName = "Starcade/DailyLogin/LootBoxChestSO")]
    public class LootBoxChestSO : ScriptableObject
    {
        public Sprite[] LootBox;

        
        public Sprite GetChestIcon(int tier)
        {
            //MainSceneLauncher.Instance.AssetLibrary.GetAsset();
            var targetIndex = Math.Clamp(tier-1, 0, int.MaxValue);
            return LootBox[targetIndex];
        }
    }
}
