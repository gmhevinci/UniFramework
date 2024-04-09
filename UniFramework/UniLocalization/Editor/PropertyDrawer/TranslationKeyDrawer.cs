using UnityEngine;
using UnityEditor;

namespace UniFramework.Localization.Editor
{
    [CustomPropertyDrawer(typeof(TranslationKeyAttribute))]
    public class TranslationKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect left = position; left.xMax -= 40;
            Rect right = position; right.xMin = left.xMax + 2;
            Color color = GUI.color;

            Rect rowPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect foldoutRect = new Rect(rowPosition.x, rowPosition.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect dropDownPosition = new Rect(foldoutRect.xMax, rowPosition.y, rowPosition.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            rowPosition.MoveToNextLine();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(left, property);
            EditorGUI.EndDisabledGroup();

            GUI.color = color;
            if (GUI.Button(right, "List") == true)
            {
                ShowPicker(dropDownPosition, property);
            }
        }

        private void ShowPicker(Rect dropDownPosition, SerializedProperty property)
        {
            var treeSelection = new TranslationKeyTreeView((key) =>
            {
                Debug.Log(key);
                if(string.IsNullOrEmpty(key) == false)
                {
                    property.stringValue = key;
                    property.serializedObject.ApplyModifiedProperties();
                }
            });

            PopupWindow.Show(dropDownPosition, new TreeViewPopupWindow(treeSelection) { Width = dropDownPosition.width });
        }
    }
}