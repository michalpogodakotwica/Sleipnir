using System;
using UnityEngine;

namespace Sleipnir.Editor
{
    public static class NodeExtensions
    {
        public const float HeaderHeight = 44f;
        public const float ResizeZoneWidth = 5f;
        
        internal static void Move(this Node node, Vector2 delta)
        {
            var rect = node.SerializedNodeData.GridRect;
            node.SerializedNodeData.GridRect = new Rect(rect.position + delta, rect.size);
        }

        internal static void Resize(this Node node, NodeResizeSide side, float delta)
        {
            const float minNodeWidth = SerializedNodeData.MinNodeWidth;
            var rectWidth = node.SerializedNodeData.GridRect.width;
            switch (side)
            {
                case NodeResizeSide.Left:
                    if (rectWidth - delta < minNodeWidth)
                        delta = rectWidth - minNodeWidth;
                    node.Move(new Vector2(delta, 0));
                    delta = -delta;
                    break;

                case NodeResizeSide.Right:
                    if (node.SerializedNodeData.GridRect.width + delta < minNodeWidth)
                        delta = minNodeWidth - rectWidth;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
            node.SerializedNodeData.GridRect = new Rect(
                node.SerializedNodeData.GridRect.position,
                node.SerializedNodeData.GridRect.size + new Vector2(delta, 0)
                );
        }

        internal static void SetNodeContentHeight(this Node node, float contentHeight)
        {
            var rect = node.SerializedNodeData.GridRect;
            node.SerializedNodeData.GridRect = new Rect(
                rect.position, 
                new Vector2(rect.width, contentHeight + HeaderHeight)
                );
        }

        internal static Rect HeaderRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect; 
            return new Rect(rect.position.x, rect.position.y, rect.width, HeaderHeight);
        }

        internal static Rect TopRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return new Rect(rect.position, new Vector2(rect.width, HeaderHeight));
        }

        internal static Rect ContentRect(this Node node)
        {
            var rect = node.SerializedNodeData.GridRect;
            return new Rect(rect.x, rect.y + HeaderHeight, rect.width, rect.height - HeaderHeight);
        }

        internal static Rect RightResizeZone(this Node serializedNodeData)
        {
            return new Rect(serializedNodeData.HeaderRect().xMax - ResizeZoneWidth, serializedNodeData.HeaderRect().y,
                ResizeZoneWidth * 2, HeaderHeight);
        }

        internal static Rect LeftResizeZone(this Node serializedNodeData)
        {
            return new Rect(serializedNodeData.HeaderRect().position.x - ResizeZoneWidth,
                serializedNodeData.HeaderRect().y, ResizeZoneWidth * 2, HeaderHeight);
        }
    }
}