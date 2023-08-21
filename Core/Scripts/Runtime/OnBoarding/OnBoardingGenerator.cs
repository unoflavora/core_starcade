using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using UnityEngine;

namespace Agate.Starcade
{
    [ExecuteInEditMode]
    public class OnBoardingGenerator : MonoBehaviour
    {
        public enum SCENE_ENUM
        {
            Debug,
            Lobby,
            Plinko,
            MatchThree,
            Monstamatch,
            CoinPusher,
            CP02,
            Setting,
        }
        
        
        [Header("On Boarding Config")]
        public int OnBoardingIdState;
        public SCENE_ENUM OnBoardingScene;
        public string SavePath;

        [Header("Character")] 
        public bool HideCharacter;
        public bool FlipCharacter;
        public Transform CharacterTransform;
        public CharacterExpression CharacterExpression;
        public Sprite CharacterSprite;
        public int ExpressionId;
        
        [Header("Dialog")]
        public bool HideDialog;
        [TextArea(5,10)]public string TextDialog;
        public Transform DialogTransform;
        public Vector2 AnchoredPosition;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        [Range(0.1f, 1f)] public float DialogWidthRatio = 0.5f;

        [Header("Highlighted Object")] 
        public Highlight Highlight;
        public Highlight.InteractType InteractType;
        public int Delay;
    }
}
