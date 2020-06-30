using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private Node _selectedNode;
        private Node _resizedNode;
        private Slot _selectedSlot;
        private bool _isDragging;
        private NodeResizeSide _resizedZone;

        private void OnEditorOpen()
        {
            _selectedSlot = null;
            _selectedNode = null;
            _resizedNode = null;
            _isDragging = false;
        }

        private void ProcessInput()
        {
            if (mouseOverWindow != this)
            {
                _isDragging = false;
                _selectedNode = null;
                _resizedNode = null;
                return;
            }

            AddHoverCursorZones();
            
            if (Event.current.OnMouseDown(0, false))
                OnLeftMouseDown();

            if (Event.current.OnMouseUp(0, false))
                OnLeftMouseUp();

            if (Event.current.OnMouseDown(1, false))
                OnRightMouseDown();

            if (Event.current.OnMouseMoveDrag(false))
                OnDrag();

            if (Event.current.OnEventType(EventType.ScrollWheel))
                OnScrollWheel();
        }

        private void AddHoverCursorZones()
        {
            if (_resizedNode != null)
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height),
                    MouseCursor.ResizeHorizontal);

            if (Nodes == null)
                return;

            foreach (var node in Nodes)
            {
                if (node == null)
                    continue;
                EditorGUIUtility.AddCursorRect(GridToGUIDrawRect(node.RightResizeZone()), 
                    MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(GridToGUIDrawRect(node.LeftResizeZone()),
                    MouseCursor.ResizeHorizontal);
            }
        }

        private void OnLeftMouseDown()
        {
            var mouseGridPosition = GuiToGridPosition(Event.current.mousePosition);

            if (Nodes == null)
            {
                _isDragging = true;
                return;
            }

            foreach (var node in Nodes)
            {
                if (node == null)
                    continue;

                if (node.RightResizeZone().Contains(mouseGridPosition))
                {
                    _resizedNode = node;
                    _resizedZone = NodeResizeSide.Right;
                    return;
                }

                if (node.LeftResizeZone().Contains(mouseGridPosition))
                {
                    _resizedNode = node;
                    _resizedZone = NodeResizeSide.Left;
                    return;
                }

                if (node.HeaderRect().Contains(mouseGridPosition))
                {
                    _selectedNode = node;
                    return;
                }

                if (node.ContentRect().Contains(mouseGridPosition))
                {
                    _selectedSlot = null;
                    return;
                }

                if (node.Slots.Any(k => k.GridRect.Contains(mouseGridPosition)))
                    return;
            }

            _isDragging = true;
        }

        private void OnLeftMouseUp()
        {
            _isDragging = false;
            _resizedNode = null;
            _selectedNode = null;
        }

        private void OnRightMouseDown()
        {
            if (_selectedNode != null || _selectedSlot != null || Nodes == null)
            {
                _selectedNode = null;
                _selectedSlot = null;
                return;
            }

            var mouseGridPosition = GuiToGridPosition(Event.current.mousePosition);
            
            foreach (var node in Nodes.Where(node => node != null))
            {
                if (node.HeaderRect().Contains(mouseGridPosition))
                {
                    ShowNodeContextMenu(node);
                    return;
                }
                
                if (node.ContentRect().Contains(mouseGridPosition))
                    return;
            }
            
            if (_graph.Connections() != null)
                foreach (var connection in _graph.Connections())
                    if (connection.IsPointOverConnection(mouseGridPosition))
                    {
                        ShowConnectionContextMenu(connection);
                        return;
                    }

            ShowGridContextMenu(mouseGridPosition);
        }

        private void ShowNodeContextMenu(Node node)
        {
            var menu = new GenericMenu();
            if (node.ContextMenuFunctions != null)
                foreach (var function in node.ContextMenuFunctions)
                    menu.AddItem(new GUIContent(function.Item1), false, () => function.Item2());
            menu.AddItem(new GUIContent("Delete Node"), false, () => RemoveNode(node));
            menu.ShowAsContext();
        }

        private void RemoveNode(Node node)
        {
            var index = _graph.Nodes.ToList().FindIndex(n => ReferenceEquals(n, node));
            
            _graph.RemoveNode(node);

            // Node hasn't been removed
            if (_graph.Nodes.Any(o => ReferenceEquals(node, o)))
                return;

            // It was the last node. This is kinda tricky - data about this last node still exists, 
            // so if you add new node it'll have old node extended data. This behaviour can be avoided
            // by storing another counter that will keep number of data entrys that exist in memory and
            // set new node dummies counter to be equal to it - for now I don't find this necessary.
            if (index >= _graph.Nodes.Count)
                return;

            if (index == 0)
                _graph.Nodes[0].SerializedNodeData.NumberOfPrecedingDummies +=
                    node.SerializedNodeData.NumberOfPrecedingDummies;
            _graph.Nodes[index].SerializedNodeData.NumberOfPrecedingDummies++;
        }

        private void ShowConnectionContextMenu(Connection connection)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Connection"), false,
                () => _graph.RemoveConnection(connection));
            menu.ShowAsContext();
        }

        private void ShowGridContextMenu(Vector2 mouseGridPosition)
        {
            var menu = new GenericMenu();
            var availableNodes = _graph.AvailableNodes();
            if (availableNodes == null)
                return;

            foreach (var nodeName in availableNodes)
                menu.AddItem(new GUIContent("Create Node/" + nodeName), false,
                    () =>
                    {
                        _graph.AddNode(nodeName);
                        Nodes[Nodes.Count - 1].Move(mouseGridPosition);
                    });

            menu.ShowAsContext();
        }
        
        private void OnDrag()
        {
            var delta = Event.current.delta * Zoom;

            if (_selectedNode != null)
                _selectedNode.Move(delta);
            else if (_isDragging)
                Pan += delta;
            else
                _resizedNode?.Resize(_resizedZone, delta.x);
        }

        private void OnScrollWheel()
        {
            UpdateZoom(Event.current.delta.y * ZoomSpeed);
        }

        public void OnSlotButtonClick(Slot slot)
        {
            if (_selectedSlot == null || _selectedSlot.Direction == slot.Direction)
                _selectedSlot = slot;
            else
            {
                _graph.AddConnection(slot.Direction == SlotDirection.Input
                    ? new Connection(_selectedSlot, slot)
                    : new Connection(slot, _selectedSlot));
                _selectedSlot = null;
            }
        }
    }
}