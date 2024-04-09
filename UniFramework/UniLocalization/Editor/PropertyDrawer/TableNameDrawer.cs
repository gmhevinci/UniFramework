using UnityEngine;
using UnityEditor;

namespace UniFramework.Localization.Editor
{
    [CustomPropertyDrawer(typeof(TableNameAttribute))]
    public class TableNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect left = position; left.xMax -= 40;
            Rect right = position; right.xMin = left.xMax + 2;
            Color color = GUI.color;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(left, property);
            EditorGUI.EndDisabledGroup();

            GUI.color = color;
            if (GUI.Button(right, "List") == true)
            {
                ShowMenu(right, property);
            }
        }

        private void ShowMenu(Rect right, SerializedProperty property)
        {
            var menu = new GenericMenu();
            foreach (var tableName in LocalizationSettingData.Setting.TableNames)
            {
                menu.AddItem(new GUIContent(tableName), property.stringValue == tableName, () =>
                {
                    property.stringValue = tableName;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            if (menu.GetItemCount() > 0)
            {
                menu.DropDown(right);
            }
            else
            {
                Debug.LogWarning($"Not found any table names ! Please check {nameof(LocalizationSetting)}");
            }
        }
    }
}