using UnityEngine;
using UnityEditor;

namespace UniFramework.Localization.Editor
{
    [CustomPropertyDrawer(typeof(TranslationKeyAttribute))]
    public class TranslationKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var left = position; left.xMax -= 40;
            var right = position; right.xMin = left.xMax + 2;
            var color = GUI.color;

            EditorGUI.PropertyField(left, property);

            GUI.color = color;
            if (GUI.Button(right, "List") == true)
            {
            }
        }
    }
}

