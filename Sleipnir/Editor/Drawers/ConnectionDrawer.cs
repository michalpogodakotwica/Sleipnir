using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public class ConnectionDrawer: OdinValueDrawer<Connection>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUIHelper.CurrentWindow.GetType() != typeof(GraphEditor))
            {
                CallNextDrawer(label);
                return;
            }
            
            var editor = (GraphEditor)GUIHelper.CurrentWindow;
            var connection = (Connection)Property.ValueEntry.WeakSmartValue;
            connection.Draw(editor);
        }
    }

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class EditorConnectionListDrawer<TListType>
        : OdinValueDrawer<TListType> where TListType : IEnumerable<Connection>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var propertyChild in ValueEntry.Property.Children)
                propertyChild.Draw();
        }
    }
}