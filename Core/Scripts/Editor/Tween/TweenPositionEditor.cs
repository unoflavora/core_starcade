using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Agate.Starcade
{
    [CustomEditor(typeof(SlideInTween))]
    public class TweenPositionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SlideInTween myScript = (SlideInTween)target;
            if(GUILayout.Button("Set Current Position As Start Point"))
            {
                myScript.StartPosition = myScript.gameObject.GetComponent<Transform>().localPosition;
            }
            if(GUILayout.Button("Set Current Position As End Point"))
            {
                myScript.EndPosition = myScript.gameObject.GetComponent<Transform>().localPosition;
            }
            if(GUILayout.Button("Move Object To Start Point"))
            {
                myScript.gameObject.GetComponent<Transform>().localPosition = myScript.StartPosition;
            }
            if(GUILayout.Button("Move Object To End Point"))
            {
                myScript.gameObject.GetComponent<Transform>().localPosition = myScript.EndPosition;
            }
        }
    }
}
