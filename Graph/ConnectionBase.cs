using System.Collections.Generic;
using Sleipnir;

namespace Graph
{
    public abstract class ConnectionBase
    {
#if UNITY_EDITOR
        public Slot Slot { get; private set; }
        
        public void Load(Node node)
        {
            if (Slot == null)
                Slot = new Slot(SlotDirection.Output, node, 17);
        }

        public bool Interactable
        {
            get => Slot.Interactable;
            set => Slot.Interactable = value;
        }
        
        public void SetPosition(float position)
        {
            Slot.RelativeYPosition = position;
        }

        public abstract IEnumerable<Connection> SleipnirConnections(IReadOnlyList<Slot> inputSlots);

        public abstract void OnNodeRemove(int index);
        public abstract bool AddConnection(Connection connection, int inputIndex);
        public abstract bool RemoveConnection(Connection connection);
#endif
    }
}