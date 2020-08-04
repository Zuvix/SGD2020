using System;
using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExtendedEditorWindow : EditorWindow
    {
        [SerializeField] protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;
        
        private string _selectedPropertyPath;
        protected int buttonIndex = -1;
        protected SerializedProperty selectedProperty;
        
        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            var lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) {continue;}
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        protected void DrawerSidebar(SerializedProperty prop)
        {
            if (prop.isArray)
            {
                for (var i = 0; i < prop.arraySize; i++)
                {
                    var p = prop.GetArrayElementAtIndex(i);
                    if (GUILayout.Toggle(buttonIndex == i, p.displayName.Replace("Element", "Level"),
                        new GUIStyle(GUI.skin.button)))
                    {
                        if (buttonIndex != i)
                            OnSelectionChange();
                        Select(p, i);
                    }
                }

                if (prop.arraySize == 0)
                {
                    buttonIndex = -1;
                }
            }

            if (!string.IsNullOrEmpty(_selectedPropertyPath))
            {
                selectedProperty = serializedObject.FindProperty(_selectedPropertyPath);
            }
        }

        protected void Select(SerializedProperty p, int newIndex)
        {
            buttonIndex = newIndex;
            _selectedPropertyPath = p.propertyPath;
        }

        protected virtual void OnSelectionChange() { }
        
        protected void DrawField(string propName, bool relative)
        {
            if (relative && currentProperty != null)
            {
                EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
            } else if (currentProperty != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
            }
        }

        protected void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
