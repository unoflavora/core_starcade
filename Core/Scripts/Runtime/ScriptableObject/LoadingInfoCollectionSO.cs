using System;
using System.Collections.Generic;
using UnityEngine;


namespace Agate.Starcade.Arcade.Plinko
{
    [Serializable]
    [CreateAssetMenu(menuName = "Loading Info/Loading Info Collection")]
    public class LoadingInfoCollectionSO : ScriptableObject
    {
        public string CollectionLabel;
        public List<LoadingInfoData> LoadingInfoDatas ;
    }
}