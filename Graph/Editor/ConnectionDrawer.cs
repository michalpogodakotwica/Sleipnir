using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sleipnir.Editor;
using UnityEditor;
using UnityEngine;

namespace Graph.Editor
{
    [DrawerPriority(0, 0, 1)]
    public class ConnectionDrawer<T> : OdinValueDrawer<T> where T : ConnectionBase
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            
            var propertyRect = EditorGUILayout.GetControlRect(false, 0);
            if (Event.current.type == EventType.Repaint)
            {
                ValueEntry.SmartValue.SetPosition(propertyRect.y + 50);
                ValueEntry.SmartValue.Interactable = true;
            }

            var textStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.MiddleRight
            };
            
            EditorGUILayout.LabelField(label == null ? "" : label.text, textStyle);
        }
    }
}