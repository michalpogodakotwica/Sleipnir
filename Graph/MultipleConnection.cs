using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sleipnir;
using UnityEngine;

namespace Graph
{
    [Serializable]
    [DisableContextMenu]
    [HideReferenceObjectPicker]
    public sealed class MultipleConnection : ConnectionBase
    {
        [SerializeField]
        [HideInInspector]
        private int[] _following = new int[0];

        internal IEnumerable<int> Following => _following;
#if UNITY_EDITOR
        private Connection[] _connections;
        public override void OnNodeRemove(int index)
        {
            var toRemove = -1;
            for (var i = 0; i < _following.Length; i++)
            {
                if (_following[i] == index)
                    toRemove = i;
                else if (_following[i] > index)
                    _following[i]--;
            }

            if (toRemove != -1)
            {
                var listed = _following.ToList();
                listed.RemoveAt(toRemove);
                _following = listed.ToArray();
                var listedConnections = _connections.ToList();
                listedConnections.RemoveAt(toRemove);
                _connections = listedConnections.ToArray();
            }
        }
        
        public override IEnumerable<Connection> SleipnirConnections(IReadOnlyList<Slot> inputSlots)
        {
            if (_connections == null)
            {
                _connections = new Connection[_following.Length];
                for (int i = 0; i < _following.Length; i++)
                    _connections[i] = new Connection(Slot, inputSlots[_following[i]]);
            }

            return _connections;
        }

        public override bool AddConnection(Connection connection, int inputIndex)
        {
            if (!ReferenceEquals(connection.OutputSlot, Slot))
                return false;

            if (_following.Contains(inputIndex))
                return true;

            var listed = _following.ToList();
            listed.Add(inputIndex);
            _following = listed.ToArray();

            var listedConnections = _connections.ToList();
            listedConnections.Add(connection);
            _connections = listedConnections.ToArray();
            return true;
        }

        public override bool RemoveConnection(Connection connection)
        {
            if (!ReferenceEquals(connection.OutputSlot, Slot))
                return false;

            var toRemove = Array.IndexOf(_connections, connection);
            var listed = _following.ToList();
            listed.RemoveAt(toRemove);
            _following = listed.ToArray();

            var listedConnections = _connections.ToList();
            listedConnections.RemoveAt(toRemove);
            _connections = listedConnections.ToArray();
            return true;
        }
#endif
    }
}