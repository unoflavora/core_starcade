using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Agate.Starcade
{
    [CustomEditor(typeof(OnBoardingGenerator))]
    public class EditorOnBoarding : UnityEditor.Editor
    {
        public OnBoardingEvent LoadedData;
        private string[] _listHighlightObject;
        private int _selectedHighlightedObject;
        private List<string> listName;

        private RectTransformData rectTransformData;

        [ContextMenu("Update List")]
        public void UpdateList()
        {
            OnBoardingGenerator myScript = (OnBoardingGenerator)target;
            listName = new List<string>();
            listName.Add("No Object");
            foreach (var gameObject in myScript.Highlight.ListHighlightedObject)
            {
                listName.Add(gameObject.name);
            }
            _listHighlightObject = listName.Select(i => i).ToArray();
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            OnBoardingGenerator myScript = (OnBoardingGenerator)target;

            if (_listHighlightObject == null)
            {
                listName = new List<string>();
                listName.Add("No Object");
                foreach (var gameObject in myScript.Highlight.ListHighlightedObject)
                {
                    listName.Add(gameObject.name);
                }
                _listHighlightObject = listName.Select(i => i).ToArray();
            }
            _selectedHighlightedObject = EditorGUILayout.Popup("Highlighted Object", _selectedHighlightedObject, _listHighlightObject); 
            if (GUILayout.Button("Update List Interactable object"))
            {
                UpdateList();
            }
            
            GUILayout.Space(50);
            
            GUILayout.Label("Dialog Debug");

            if (GUILayout.Button("Update Anchor Position"))
            {
                myScript.AnchoredPosition = myScript.DialogTransform.GetComponent<RectTransform>().anchoredPosition;
                myScript.AnchorMin = myScript.DialogTransform.GetComponent<RectTransform>().anchorMin;
                myScript.AnchorMax = myScript.DialogTransform.GetComponent<RectTransform>().anchorMax;
            }
            
            if (GUILayout.Button("Update Dialog Position And Size for Landscape"))
            {
                myScript.DialogTransform.gameObject.GetComponent<DialogBox>().SetDialogDebugLandscape(myScript.TextDialog,
                    myScript.DialogWidthRatio, myScript.AnchoredPosition, myScript.AnchorMin, myScript.AnchorMax);
                myScript.DialogTransform.gameObject.SetActive(false);
                myScript.DialogTransform.gameObject.SetActive(true);
            }
            
            if (GUILayout.Button("Update Dialog Position And Size for Portrait"))
            {
                myScript.DialogTransform.gameObject.GetComponent<DialogBox>().SetDialogDebugPortrait(myScript.TextDialog,
                    myScript.DialogWidthRatio, myScript.AnchoredPosition, myScript.AnchorMin, myScript.AnchorMax);
                myScript.DialogTransform.gameObject.SetActive(false);
                myScript.DialogTransform.gameObject.SetActive(true);
            }
            
            GUILayout.Space(50);

            if (GUILayout.Button("Generate On Boarding Event"))
            {
                OnBoardingEvent newOnBoardingEvent = new OnBoardingEvent();
                
                //SET CHARACTER DATA
                newOnBoardingEvent.HideCharacter = myScript.HideCharacter;
                newOnBoardingEvent.CharacterExpression = myScript.CharacterExpression;
                newOnBoardingEvent.CharacterExpressionId = myScript.ExpressionId;
                RectTransformData rectTransformDataChara = new RectTransformData();
                rectTransformDataChara.PullFromTransform(myScript.CharacterTransform.GetComponent<RectTransform>());
                newOnBoardingEvent.CharacterAnchoredPosition = rectTransformDataChara.AnchoredPosition;
                

                //SET DIALOG DATA
                newOnBoardingEvent.HideDialog = myScript.HideDialog;
                newOnBoardingEvent.DialogText = myScript.TextDialog;
                newOnBoardingEvent.DialogWidthRatio = myScript.DialogWidthRatio;
                RectTransformData rectTransformData = new RectTransformData();
                
                rectTransformData.PullFromTransform(myScript.DialogTransform.GetComponent<RectTransform>());
                newOnBoardingEvent.DialogAnchoredPosition = rectTransformData.AnchoredPosition;
                newOnBoardingEvent.DialogAnchorMin = rectTransformData.AnchorMin;
                newOnBoardingEvent.DialogAnchorMax = rectTransformData.AnchorMax;

                //SET HIGHLIGHTED DATA
                newOnBoardingEvent.HighlightedObjectIndex = _selectedHighlightedObject - 1;
                Debug.Log(_selectedHighlightedObject - 1);
                newOnBoardingEvent.InteractType = myScript.InteractType;
                newOnBoardingEvent.Delay = myScript.Delay;
                
                //SAVE DATA
                string scene = Enum.GetName(typeof(OnBoardingGenerator.SCENE_ENUM), myScript.OnBoardingScene);
                string objectName = "Event_" + scene + "_" + myScript.OnBoardingIdState;
                string path = "Assets/Starcade/Scripts/ScriptableObject/FTUE/Event/" + scene + "/" + objectName + ".asset";
                if (File.Exists(path))
                {
                    Debug.LogError("FILE ALREADY EXIST");
                    return;
                }
                AssetDatabase.CreateAsset(newOnBoardingEvent,path);
                Debug.Log("SUCCESS CREATING AT : " + path);
            }
            
            EditorGUILayout.Space(10f);
            // LoadedData = (OnBoardingEvent)EditorGUILayout.ObjectField("Load Data", LoadedData, typeof(OnBoardingEvent), true);
            // if (GUILayout.Button("Load Data to Current Field"))
            // {
            //     myScript.Character = LoadedData.Character;
            //     LoadedData = null;
            // }
        }
    }
}
