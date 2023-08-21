using System;
using Agate.Starcade.Arcade.Plinko;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Agate.Starcade.Arcade.Plinko
{
    [CreateAssetMenu(menuName = "Starcade Scriptable Object/BoardData")]
    public class BoardData : ScriptableObject
    {
        public GoalData goalData;
        public PinData pinData;
        public SymbolData symbolData;
    }

    [Serializable]
    public class GoalData
    {
        public List<float> goalCoinReward;
        public List<float> goalJackpotReward;
        
    }
    
    [Serializable]
    public class SymbolData
    {
        public List<string> symbol;
        public List<bool>  isStar;
    }
    
      [Serializable]
        public class PinData
        {
            public int pinHeight;
            [Multiline(40)]
            public string pinMap;
            public List<int >pinType;
        }
}