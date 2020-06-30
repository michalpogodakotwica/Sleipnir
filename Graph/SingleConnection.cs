using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sleipnir;
using UnityEngine;

namespace Graph
{
    [Serializable]
    [DisableContextMenu]
    [HideReferenceObjectPicker]
    public sealed class SingleConnection : ConnectionBase
    {
        [SerializeField]
        [HideInInspector]
        private int _following = -1;

        internal int Following => _following;

        public bool HasFollowing => _following != -1;
        
#if UNITY_EDITOR
        private Connection? _connection;
        
        public override IEnumerable<Connection> SleipnirConnections(IReadOnlyList<Slot> inputSlots)
        {
            if (_following == -1)
                yield break;

            if(!_connection.HasValue && _following != -1)
                _connection = new Connection(Slot, inputSlots[_following]);

            yield return _connection.Value;
        }

        public override void OnNodeRemove(int index)
        {
            if (_following == index)
            {
                _following = -1;
                _connection = null;
            }
            else if (_following > index)
                _following--;
        }

        public override bool AddConnection(Connection connection, int inputIndex)
        {
            if (!ReferenceEquals(connection.OutputSlot, Slot))
                return false;

            _following = inputIndex;
            _connection = connection;
            return true;
        }

        public override bool RemoveConnection(Connection connection)
        {
            if (!ReferenceEquals(connection.OutputSlot, Slot))
                return false;

            _following = -1;
            _connection = null;
            return true;
        }
#endif
    }
}