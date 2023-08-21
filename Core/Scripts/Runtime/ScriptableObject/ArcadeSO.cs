using System;
using Agate.Starcade;
using Agate.Starcade.Boot;
using UnityEngine;
using UnityEngine.Video;
using Agate;

[Serializable]
[CreateAssetMenu(menuName = "Starcade Scriptable Object/ Arcade SO")]
public class ArcadeSO : ScriptableObject
{
    public string ArcadeName;
    public string ArcadeSlug;
    public string ArcadeDescription;
    public Sprite ArcadeIcon;
    public Sprite ArcadeMachine;
    public VideoClip ArcadeVideo;
    public AddressableController.AssetID AssetId;
    public SceneData ArcadeSceneData;
    public bool IsHaveTutorial;
    public bool IsLocked;
}
