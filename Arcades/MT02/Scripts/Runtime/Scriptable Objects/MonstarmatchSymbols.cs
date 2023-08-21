using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Hexacore.Symbols;
using Starcade.Arcades.MT02.Scripts.Runtime;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Monstamatch Scriptable Object / Symbol Data SO")]
    public class MonstarmatchSymbols : UnityEngine.ScriptableObject
    {
       [SerializeField] public List<MonstamatchSymbolData> symbols;
    }
}
