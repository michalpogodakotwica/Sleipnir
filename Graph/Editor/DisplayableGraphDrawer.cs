using Sirenix.OdinInspector.Editor;
using Sleipnir.Editor;
using UnityEditor;
using UnityEngine;

namespace Graph.Editor
{
    [DrawerPriority(0, 0, 10)]
    public class DisplayableGraphDrawer<TGraph, TNode, TNodeContent> : OdinValueDrawer<TGraph>
        where TGraph : DisplayableGraph<TNode, TNodeContent>
        where TNode : Node<TNodeContent>, new()
        where TNodeContent : class, INode
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUILayout.Button("Open editor"))
            {
                var window = (GraphEditor) EditorWindow.GetWindow(typeof(GraphEditor));
                ValueEntry.SmartValue.Load();
                
                var serializationRoot = ValueEntry.Property.SerializationRoot;
                window.LoadGraph(ValueEntry.SmartValue, (Object) serializationRoot.ValueEntry.WeakSmartValue);
            }
            
            foreach (var child in Property.Children)
                child.Draw(label);
        }
    }
}