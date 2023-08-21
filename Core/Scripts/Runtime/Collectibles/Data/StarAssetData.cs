using System;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Data
{
    
    [CreateAssetMenu(menuName = "Starcade Scriptable Object/StarPinSO", fileName = "Star Asset Data", order = 1)]
    public class StarAssetData : ScriptableObject
    {
        [SerializeField] public Transform availableStar;
        [SerializeField] public Transform unavailableStar;
    }
}