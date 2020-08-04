using System;
using Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId) as LevelDataObject;
            if (obj != null)
            {
                LevelEditorWindow.Open(obj);
                return true;
            }
            return false;
        }
    }
    
    [CustomEditor(typeof(LevelDataObject))]
    public class CustomLevelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Level Editor"))
            {
                LevelEditorWindow.Open((LevelDataObject)target);
            }
            EditorGUILayout.Space();
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}
