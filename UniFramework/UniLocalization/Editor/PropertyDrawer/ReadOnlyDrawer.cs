using System;
using UnityEditor;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label);
            EditorGUI.BeginDisabledGroup(true);      
            EditorGUI.PropertyField(position, property);
            EditorGUI.EndDisabledGroup();
        }
    }
}

