using Agate.Starcade.Boot;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Agate.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public class SceneReferenceAttributeDrawer : PropertyDrawer
    {
        private const string INVALID_PROPERTY = "ERROR : Scene Reference Attribute can only be used on string";

        private bool isValidProperty;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1 + (isValidProperty ? 1 : 0);
            return EditorGUIUtility.singleLineHeight * lineCount;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                isValidProperty = false;
                EditorGUI.LabelField(position, INVALID_PROPERTY);
                return;
            }
            isValidProperty = true;
            Rect objFieldPos = new Rect(position.x, position.y, position.width, 16f);
            Rect serializeLabelPos = new Rect(position.x, position.y + 16f, position.width, 16f);

            EditorGUI.BeginProperty(position, label, property);

            SceneAsset parsedSceneAsset = ParseScenePath(property.stringValue);
            SceneAsset selectedSceneAsset = parsedSceneAsset;
            selectedSceneAsset = (SceneAsset)EditorGUI.ObjectField(objFieldPos, label.text, selectedSceneAsset,
                typeof(SceneAsset), false);
            string selectedScenePath = AssetDatabase.GetAssetPath(selectedSceneAsset);
            bool isSelectedValid = IsSceneRegistered(selectedScenePath);

            // preserve previously assigned data if selected asset ref can't be found in Player Build Setting
            string fallbackPath = selectedSceneAsset ? property.stringValue : null;
            property.stringValue = isSelectedValid ? selectedScenePath : fallbackPath;

            if (selectedSceneAsset && !isSelectedValid) Debug.LogWarning($"{selectedSceneAsset.name} isn't registered in Player Build Setting", selectedSceneAsset);

            string prefix = string.IsNullOrEmpty(property.stringValue) ? "" : "...";
            string labelText = $"{prefix}{Path.GetFileName(property.stringValue)}";

            EditorGUI.LabelField(serializeLabelPos,
                new GUIContent("    serialized as"),
                new GUIContent($"\"{labelText}\"", property.stringValue));

            EditorGUI.EndProperty();
        }

        private static SceneAsset ParseScenePath(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) return null;
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == scenePath);
            return scene == null ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
        }

        private static bool IsSceneRegistered(string scenePath)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path == scenePath);
            return scene != null;
        }
    }
#endif
}