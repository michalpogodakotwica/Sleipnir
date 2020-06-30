using System.Collections.Generic;
using UnityEngine;

namespace Sleipnir
{
    public interface IGraph
    {
#if UNITY_EDITOR
        float Zoom { get; set; }
        Vector2 Pan { get; set; }

        IList<Node> Nodes { get; }
        IEnumerable<string> AvailableNodes();
        void AddNode(string key);
        void RemoveNode(Node toRemove);

        IEnumerable<Connection> Connections();
        void AddConnection(Connection connection);
        void RemoveConnection(Connection connection);
#endif
    }
}