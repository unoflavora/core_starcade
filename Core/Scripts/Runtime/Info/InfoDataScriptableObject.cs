using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Info
{
    [Serializable]
    [CreateAssetMenu(fileName = "DictInfoData", menuName = "Dictionary Info Data", order = 0)]
    public class InfoDataScriptableObject : ScriptableObject
    {
        public List<InfoData<InfoType>> ListInfoDatas;
        public List<InfoData<ErrorType>> ListInfoErrorDatas;
        public List<InfoData<WarningType>> ListInfoWarningDatas;
    }
}