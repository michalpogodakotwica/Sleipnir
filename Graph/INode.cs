using System.Collections.Generic;

namespace Graph
{
    public interface INode
    {
        IEnumerable<ConnectionBase> VisibleConnections();
        IEnumerable<ConnectionBase> AllConnections();
    }
}