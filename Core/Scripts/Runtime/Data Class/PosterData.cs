using System;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Info;
using Agate.Starcade.Core.Runtime.Lobby;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

namespace Agate.Starcade.Scripts.Runtime.Data_Class
{
    [CreateAssetMenu(fileName = "Poster", menuName = "Poster/PosterDataSO", order = 1)]
    [Serializable]
    public class PosterData : ScriptableObject
    {
        [Header("Identity")] 
        public string PosterId;
        
        [Header("UI")]
        public Sprite PosterSprite;
        public InfoType InfoPopUpType;
        
        [Header("Action")]
        public PosterActionEnum PosterAction;
        public PosterActionData PosterActionData;

        [Header("Other")]
        public bool IsClaimableOnClickPoster;
        public bool IsComingSoon;
        public bool IsDontHaveReward;
    }

    [Serializable]
    public class PosterActionData
    {
        public string PosterActionStringData;
        public int PosterActionIntData;
        public float PosterActionFloatData;
        public AssetReference PosterActionAssetData;
        public Object PosterActionObjectData;
        [SceneReference] public string PosterActionScenePath;
    }
}