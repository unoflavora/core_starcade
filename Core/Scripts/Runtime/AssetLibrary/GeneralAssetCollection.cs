using Agate.Starcade;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
[CreateAssetMenu(menuName = "Asset Library/Add General Asset Collection")]
public class GeneralAssetCollection : ScriptableObject
{
    public string Label;
    public BaseAssetReferenceData<AssetReference>[] Collection;
}

