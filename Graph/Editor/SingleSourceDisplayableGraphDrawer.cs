using Sirenix.OdinInspector.Editor;
using Sleipnir.Editor;
using UnityEditor;
using UnityEngine;

namespace Graph.Editor
{
    [DrawerPriority(0, 0, 10)]
    public class GraphDrawer<TGraph, TStartingNode, TStartingNodeContent, TNode, TNodeContent> : OdinValueDrawer<TGraph>
        where TGraph : CustomStartingPointGraph<TStartingNode, TStartingNodeContent, TNode, TNodeContent>
        where TNode : Node<TNodeContent>, new()
        where TStartingNode : Node<TStartingNodeContent>, new()
        where TNodeContent : class, INode
        where TStartingNodeContent : INode
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