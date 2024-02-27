using UnityEngine;
using UnityEditor;

namespace UniFramework.Localization.Editor
{
    [CustomPropertyDrawer(typeof(TableNameAttribute))]
    public class TableNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var left = position; left.xMax -= 40;
            var right = position; right.xMin = left.xMax + 2;
            var color = GUI.color;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(left, property);
            EditorGUI.EndDisabledGroup();

            GUI.color = color;
            if (GUI.Button(right, "List") == true)
            {
				var menu = new GenericMenu();

				foreach (var tableName in LocalizationSettingData.Setting.TableNames)
				{
					menu.AddItem(new GUIContent(tableName), property.stringValue == tableName, () => { property.stringValue = tableName; property.serializedObject.ApplyModifiedProperties(); });
				}

				if (menu.GetItemCount() > 0)
				{
					menu.DropDown(right);
				}
				else
				{
					Debug.LogWarning("Your scene doesn't contain any phrases, so the phrase name list couldn't be created.");
				}
			}
        }
    }
}

