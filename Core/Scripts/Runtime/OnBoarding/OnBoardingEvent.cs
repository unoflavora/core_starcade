using System;
using System.Collections;
using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.OnBoarding;
using UnityEngine;

namespace Agate.Starcade
{
    [Serializable]
    [CreateAssetMenu(fileName = "OnBoardingEvent", menuName = "OnBoarding/OnBoardingEvent", order = 1)]
    public class OnBoardingEvent : ScriptableObject, IListOnBoardingEvent
    {
        [Header("Character")] 
        public bool HideCharacter;
        public Vector2 CharacterAnchoredPosition;
        public CharacterExpression CharacterExpression;
        public int CharacterExpressionId;
        public bool IsFlipped;
        
        [Header("Dialog")]
        [TextArea(15,20)] public string DialogText;
        public bool HideDialog;
        [Range(0.1f, 1f)]public float DialogWidthRatio;
        public Vector2 DialogAnchoredPosition;
        public Vector2 DialogAnchorMin;
        public Vector2 DialogAnchorMax;

        [Header("Highligted Object")]
        public int HighlightedObjectIndex;
        public Highlight.InteractType InteractType;
        public int Delay = 0;
    }
}
