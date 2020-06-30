using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sleipnir;
using UnityEngine;

namespace Graph
{
    // TODO int based indexing got really messy here, it might be a good idea to clean up graphs at some point
    [Serializable]
    public class CustomStartingPointGraph<TStartingNode, TStartingNodeContent, TNode, TContent> : IGraph
        where TNode : Node<TContent>, new() // Generic constraint enables Unity serialization if needed
        where TStartingNode : Node<TStartingNodeContent>, new()
        where TStartingNodeContent : INode
        where TContent : class, INode
    {
        [FoldoutGroup("GraphData")] 
        [SerializeField] 
        [ReadOnly]
        private TNode[] _nodesWithoutStart;

        [FoldoutGroup("GraphData")]
        [SerializeField]
        [ReadOnly]
        private TStartingNode _startingNode = new TStartingNode();
        
        public TStartingNodeContent StartingNode => _startingNode.Content();

        public TContent Following(SingleConnection connection)
        {
            return connection == null || connection.Following == -1 ? null : _nodesWithoutStart[connection.Following - 1].Content();
        }
        
        public IEnumerable<TContent> Following(MultipleConnection connection)
        {
            return connection?.Following.Select(f => _nodesWithoutStart[f].Content());
        }
        
        public IEnumerable<TContent> NodesWithoutStart => _nodesWithoutStart.Select(n => n.Content()); 

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

            if (_nodesWithoutStart == null)
                _nodesWithoutStart = new TNode[0];

            if (_startingNode == null || _startingNode.Content() == null)
            {
                _startingNode = new TStartingNode();
                var content = Activator.CreateInstance(typeof(TStartingNodeContent));
                _startingNode.Create((TStartingNodeContent)content, GetNodeTitle(content.GetType()), null, _nodeStartWidth);
            }

            _startingNode.Load(GetNodeTitle(_startingNode.Content().GetType()), null);
            
            foreach (var node in _nodesWithoutStart)
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

        public IList<Node> Nodes => new[] {_startingNode.SleipnirNode()}
            .Concat(_nodesWithoutStart.Select(n => n.SleipnirNode()))
            .ToList();

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

            var listedNodes = _nodesWithoutStart.ToList();
            listedNodes.Add(node);
            _nodesWithoutStart = listedNodes.ToArray();
        }

        public virtual void RemoveNode(Node toRemove)
        {
            if(toRemove == _startingNode.SleipnirNode())
                return;

            var index = _nodesWithoutStart.Select(n => n.SleipnirNode()).ToList().IndexOf(toRemove) + 1;

            foreach (var node in _nodesWithoutStart)
                node.OnNodeRemove(index);

            var listedNodes = _nodesWithoutStart.ToList();
            listedNodes.RemoveAt(index - 1);
            _nodesWithoutStart = listedNodes.ToArray();
        }

        public IEnumerable<Connection> Connections()
        {
            var inputSlots = new[] {_startingNode.InputSlot()}
                .Concat(_nodesWithoutStart.Select(n => n.InputSlot()))
                .ToList();
            
            foreach (var connection in _startingNode.Content().VisibleConnections())
                foreach (var sleipnirConnection in connection.SleipnirConnections(inputSlots))
                    yield return sleipnirConnection;

            foreach (var sleipnirConnection in _nodesWithoutStart.Select(n => n.Content())
                                                    .Where(n => n.VisibleConnections() != null)
                                                    .SelectMany(n => n.VisibleConnections())
                                                    .SelectMany(c => c.SleipnirConnections(inputSlots)))
                yield return sleipnirConnection;
        }

        public virtual void AddConnection(Connection connection)
        {
            var inputIndex = -1;
            for (var i = 0; i < _nodesWithoutStart.Length; i++)
            {
                if (!ReferenceEquals(connection.InputSlot, _nodesWithoutStart[i].InputSlot()))
                    continue;

                inputIndex = i + 1;
                break;
            }
            
            if(_startingNode.AddConnection(connection, inputIndex))
                return;
            
            foreach (var n in _nodesWithoutStart)
                if (n.AddConnection(connection, inputIndex))
                    break;
        }

        public virtual void RemoveConnection(Connection connection)
        {
            if(_startingNode.RemoveConnection(connection))
                return;
            
            foreach (var n in _nodesWithoutStart)
                if (n.RemoveConnection(connection))
                    break;
        }
#endif
    }
}