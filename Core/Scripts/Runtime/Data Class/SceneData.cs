using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Boot;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Agate
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "SceneSO/SceneData", order = 1)]
    public class SceneData : ScriptableObject
    {
        public enum SceneOrientation
        {
            Unknown,
            Portrait,
            Landscape
        }

        public string SceneKey;
        [SceneReference] public string ScenePath;
        public AssetReference SceneReference;
        public bool IsAddressable;
        public SceneOrientation TargetSceneOrientation;
        public Sprite Background;
    }
}
