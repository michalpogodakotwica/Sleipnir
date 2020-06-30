using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class NodeDrawer : OdinValueDrawer<Node>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var editor = GUIHelper.CurrentWindow as GraphEditor;
            if (editor == null)
            {
                CallNextDrawer(label);
                return;
            }

            var node = ValueEntry.SmartValue;
            var headerGUIRect = editor.GridToGUIDrawRect(node.HeaderRect());
            var topBox = editor.GridToGUIDrawRect(node.TopRect());
            
            // Draw top box
            GUIHelper.PushColor(node.HeaderColor);
            GUI.Box(topBox, "", new GUIStyle("ProgressBarBack"));
            GUIHelper.PopColor();
            
            var titleGUIStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = node.TitleColor,
                }
            };
            GUI.Label(headerGUIRect, node.HeaderTitle, titleGUIStyle);
            
            // Draw content
            var contentRect = node.ContentRect();
            GUILayout.BeginArea(editor.GridToGUIDrawRect(new Rect(contentRect.x, contentRect.y, contentRect.width,
                contentRect.height)));
            var contentBoxRect = SirenixEditorGUI.BeginBox();

            GUIHelper.PushContextWidth(contentRect.width);
            GUIHelper.PushLabelWidth(Mathf.Max((float) (contentRect.width * 0.449999988079071 - 40.0), 120f));
            GUIHelper.PushHierarchyMode(false);
            CallNextDrawer(label);
            GUIHelper.PopHierarchyMode();
            GUIHelper.PopContextWidth();
            GUIHelper.PopLabelWidth();
            
            if (Event.current.type == EventType.Repaint)
                node.SetNodeContentHeight(contentBoxRect.height);

            SirenixEditorGUI.EndBox();
            GUILayout.EndArea();

            foreach (var slot in node.Slots)
            {
                if (slot.Visible)
                {
                    GUIHelper.PushGUIEnabled(slot.Interactable);
                    GUIHelper.PushColor(slot.Color);
                    if (GUI.Button(editor.GridToGUIDrawRect(slot.GridRect), ""))
                        editor.OnSlotButtonClick(slot);
                    GUIHelper.PopColor();
                    GUIHelper.PopGUIEnabled();
                }
            }
        }
    }

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class EditorNodeListDrawer<TListType>
        : OdinValueDrawer<TListType> where TListType : IEnumerable<Node>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var propertyChild in ValueEntry.Property.Children)
                if ((Node)propertyChild.ValueEntry.WeakSmartValue != null)
                    propertyChild.Draw();
        }
    }
}
