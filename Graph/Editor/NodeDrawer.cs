using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sleipnir.Editor;
using UnityEngine;

namespace Graph.Editor
{
    [DrawerPriority(250, 0, 0)]
    public class NodeDrawer<T> : OdinValueDrawer<T> where T : INode
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }

            if (Event.current.type == EventType.Repaint)
            {
                foreach (var connection in ValueEntry.SmartValue.AllConnections())
                    connection.Slot.Visible = false;

                foreach (var connection in ValueEntry.SmartValue.VisibleConnections())
                {
                    connection.SetPosition(17);
                    connection.Interactable = false;
                    connection.Slot.Visible = true;
                }
            }
            CallNextDrawer(label);
        }
    }
}