using Sirenix.OdinInspector;
using Sleipnir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Graph
{
    [Serializable]
    public class Node<T> where T : INode
    {
        [SerializeField]
        [HideLabel]
        [HideReferenceObjectPicker]
        private T _content;

        public T Content() => _content;

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private SerializedNodeData _serializedNodeData;

        private Node _sleipnirNode;
        private Slot _inputSlot;

        public void Create(T content, string title, List<Tuple<string, Action>> contextFunctions, float nodeWidth)
        {
            _content = content;
            _serializedNodeData = new SerializedNodeData();

            var rect = _serializedNodeData.GridRect;
            rect.width = nodeWidth;
            _serializedNodeData.GridRect = rect;

            Load(title, contextFunctions);
        }

        public void Load(string title, List<Tuple<string, Action>> contextFunctions)
        {
            _sleipnirNode = new Node(
                () => _content,
                value => _content = (T)value,
                _serializedNodeData)
            {
                HeaderTitle = title,
                ContextMenuFunctions = contextFunctions
            };

            _inputSlot = new Slot(SlotDirection.Input, _sleipnirNode, 17);
            LoadSlots();
        }

        private void LoadSlots()
        {
            var value = (INode)_sleipnirNode.ValueGetter();

            foreach (var connection in value.AllConnections())
                connection.Load(_sleipnirNode);

            _sleipnirNode.Slots = value.AllConnections().Select(c => c.Slot).ToList();
            if(_content.GetType().GetCustomAttribute<HideInputSlot>(true) == null)
                _sleipnirNode.Slots.Add(_inputSlot);
        }

        public Node SleipnirNode()
        {
            LoadSlots();    // This is working but it's inefficient. Attribute processor is more than welcomed.
            return _sleipnirNode;
        }

        public Slot InputSlot() => _inputSlot;

        public bool AddConnection(Connection sleipnirConnection, int inputIndex)
        {
            var node = (INode)Content();
            var connections = node.AllConnections();
            return connections != null && connections.Any(c => c.AddConnection(sleipnirConnection, inputIndex));
        }

        public bool RemoveConnection(Connection sleipnirConnection)
        {
            var node = (INode)Content();
            var connections = node.AllConnections();
            return connections != null && connections.Any(c => c.RemoveConnection(sleipnirConnection));
        }

        public void OnNodeRemove(int index)
        {
            var node = (INode)Content();
            var connections = node.AllConnections();
            if (connections == null)
                return;

            foreach (var connection in connections)
                connection.OnNodeRemove(index);
        }
#endif
    }
}