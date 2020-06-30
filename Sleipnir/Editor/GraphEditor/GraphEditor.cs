using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sleipnir.Editor
{        
    public partial class GraphEditor : OdinEditorWindow
    {
        private const float ZoomSpeed = 0.1f;
        private const float MaxZoom = 7f;

        private IGraph _graph;
        private Object _serializationRoot;
        
        [HideReferenceObjectPicker, HideLabel, ShowInInspector, PropertyOrder(1)]
        public List<Node> Nodes
        {
            get
            {
                return _graph.Nodes?
                    .SelectMany(n => Enumerable.Repeat<Node>(
                        null, 
                        n.SerializedNodeData.NumberOfPrecedingDummies
                        )
                    .Concat(new[] {n}))
                    .ToList();
            }
            set
            {
                // without setter property is marked as readonly
            }
        }

        [ShowInInspector, PropertyOrder(0)]
        public List<Connection> Connections => _graph.Connections()?.ToList();

        public void LoadGraph(IGraph graph, Object serializationRoot)
        {
            _serializationRoot = serializationRoot;
            _graph = graph;
            LoadTextures();
            OnEditorOpen();
        }

        public Vector2 Pan
        {
            get => _graph?.Pan ?? Vector2.zero;
            set
            {
                _graph.Pan = value;
                GUIHelper.RequestRepaint();
            }
        }

        public float Zoom
        {
            get => _graph == null || _graph.Zoom < 1 ? 1 : _graph.Zoom;
            set
            {
                _graph.Zoom = Mathf.Clamp(value, 1f, MaxZoom);
                GUIHelper.RequestRepaint();
            }
        }

        public void Drag(Vector2 delta)
        {
            Pan += delta;
        }

        private void UpdateZoom(float delta)
        {
            Zoom += delta;
        }

        private void BeginZoomed()
        {
            GUI.EndClip();
            GUI.EndClip();
            
            GUIUtility.ScaleAroundPivot(Vector2.one / Zoom, position.size * 0.5f);
            GUI.BeginClip(new Rect(-(position.width * Zoom - position.width) * 0.5f,
                -((position.height * Zoom - position.height) * 0.5f) + 22 * Zoom,
                position.width * Zoom,
                position.height * Zoom));
        }

        private void EndZoomed()
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * Zoom, position.size * 0.5f);
            GUI.BeginClip(GUIHelper.CurrentWindow.position);
        }

        public Vector2 GuiToGridPosition(Vector2 guiPosition)
        {
            return (guiPosition - position.size * 0.5f - Pan / Zoom) * Zoom;
        }

        public Vector2 GridToGuiPosition(Vector2 gridPosition)
        {
            return position.size * 0.5f + Pan / Zoom + gridPosition / Zoom;
        }

        public Rect GridToGUIDrawRect(Rect gridRect)
        {
            gridRect.position = GridToGuiPositionNoClip(gridRect.position);
            return gridRect;
        }

        public Vector2 GridToGuiPositionNoClip(Vector2 gridPosition)
        {
            return position.size * 0.5f * Zoom + Pan + gridPosition;
        }
    }
}