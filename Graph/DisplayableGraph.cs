using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using Sleipnir;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Graph
{
    [Serializable]
    public class DisplayableGraph<TNode, TContent> : IGraph, IReadOnlyList<TContent> 
        where TNode : Node<TContent>, new() // Generic constraint enables Unity serialization if needed
        where TContent : class, INode
    {
        [FoldoutGroup("GraphData")]
        [SerializeField]
        [ReadOnly]
        protected TNode[] _nodes = new TNode[0];
        
        public IEnumerator<TContent> GetEnumerator()
        {
            return _nodes.Select(n => n.Content()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _nodes.Length;
        public TContent this[int index] => index == -1 ? null : _nodes[index].Content();
        
        public TContent Following(SingleConnection connection)
        {
            return connection == null || connection.Following == -1 ? null : _nodes[connection.Following].Content();
        }
        
        public IEnumerable<TContent> Following(MultipleConnection connection)
        {
            return connection?.Following.Select(f => _nodes[f].Content());
        }

#if UNITY_EDITOR
        [FoldoutGroup("SleipnirData", order : -1)]
        [SerializeField]
        private float _zoom;
        
        [FoldoutGroup("SleipnirData")]
        [SerializeField]
        private Vector2 _pan;
        
        [FoldoutGroup("SleipnirData")]
        [SerializeField]
        protected float _nodeStartWidth = 200;

        protected Dictionary<string, Type> NodeTypes;

        public virtual void Load()
        {
            NodeTypes = GetNodeTypes();

            if (_nodes == null)
                _nodes = new TNode[0];

            foreach (var node in _nodes)
                node.Load(GetNodeTitle(node.Content().GetType()), null);
        }

        public Vector2 Pan
        {
            get => _pan;
            set => _pan = value;
        }

        public float Zoom
        {
            get => _zoom;
            set => _zoom = value;
        }

        public IList<Node> Nodes => _nodes.Select(n => n.SleipnirNode()).ToList();

        public IEnumerable<string> AvailableNodes()
        {
            return NodeTypes.Select(t => t.Key);
        }

        protected Dictionary<string, Type> GetNodeTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(TContent).IsAssignableFrom(t) && !t.IsAbstract);

            return types.ToDictionary(GetNodePath, t => t);
        }

        private static string GetNodePath(Type type)
        {
            var nodePathAttribute = type.GetCustomAttribute<NodePathAttribute>(true);
            return nodePathAttribute != null ? nodePathAttribute.Path : type.GetNiceName();
        }

        protected static string GetNodeTitle(Type type)
        {
            var titleAttribute = type.GetCustomAttribute<NodeTitleAttribute>(true);
            return titleAttribute != null ? titleAttribute.Title : type.GetNiceName();
        }

        public virtual void AddNode(string key)
        {
            var type = NodeTypes[key];
            var content = Activator.CreateInstance(type);
            var node = new TNode();
            node.Create((TContent) content, GetNodeTitle(content.GetType()), null, _nodeStartWidth);

            var listedNodes = _nodes.ToList();
            listedNodes.Add(node);
            _nodes = listedNodes.ToArray();
        }

        public virtual void RemoveNode(Node toRemove)
        {
            var index = Nodes.IndexOf(toRemove);

            foreach (var node in _nodes)
                node.OnNodeRemove(index);

            var listedNodes = _nodes.ToList();
            listedNodes.RemoveAt(index);
            _nodes = listedNodes.ToArray();
        }

        public IEnumerable<Connection> Connections()
        {
            var inputSlots = _nodes.Select(n => n.InputSlot()).ToList();
            return _nodes
                    .Select(n => n.Content())
                    .Where(n => n.VisibleConnections() != null)
                    .SelectMany(n => n.VisibleConnections())
                    .SelectMany(c => c.SleipnirConnections(inputSlots));
        }

        public virtual void AddConnection(Connection connection)
        {
            var inputIndex = -1;
            for (var i = 0; i < _nodes.Length; i++)
            {
                if (!ReferenceEquals(connection.InputSlot, _nodes[i].InputSlot()))
                    continue;

                inputIndex = i;
                break;
            }

            foreach (var n in _nodes)
                if (n.AddConnection(connection, inputIndex))
                    break;
        }

        public virtual void RemoveConnection(Connection connection)
        {
            foreach (var n in _nodes)
                if (n.RemoveConnection(connection))
                    break;
        }
#endif
    }
}