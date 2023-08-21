using System;
using Agate.Starcade;
using Agate.Starcade.Boot;
using UnityEngine;
using UnityEngine.Video;
using Agate;

[Serializable]
[CreateAssetMenu(menuName = "Starcade Scriptable Object/ Asset SO")]
public class AssetSO : ScriptableObject
{
    public AddressableController.AssetID Id;
    public string SceneKey;
    public string ActiveKey;
    public string[] AssetKeys;
}
