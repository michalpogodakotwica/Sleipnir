using UnityEngine;

namespace Sleipnir
{
    public class Slot
    {
#if UNITY_EDITOR
        public static readonly Vector2 SlotButtonSize = new Vector2(12, 12);

        public readonly SlotDirection Direction;
        public readonly Node Node;

        public float RelativeYPosition;
        public Color Color;
        public float HorizontalPadding;
        public bool Interactable;
        
        // TODO slot visibility shouldn't be controlled by a flag
        // This hack enables easy knobs caching but it's not safe and should be removed.
        public bool Visible;

        public Rect GridRect
        {
            get
            {
                var nodeRect = Node.SerializedNodeData.GridRect;
                var yPosition = nodeRect.y + RelativeYPosition;
                var xPosition = Direction == SlotDirection.Input
                    ? nodeRect.x - SlotButtonSize.x - HorizontalPadding
                    : nodeRect.xMax + HorizontalPadding;

                var knobPosition = new Vector2(xPosition, yPosition);
                return new Rect(knobPosition, SlotButtonSize);
            }
        }

        public Slot(SlotDirection direction, Node node, float relativeYPosition)
        {
            Direction = direction;
            Node = node;

            RelativeYPosition = relativeYPosition;
            Color = new Color(0.95f, 0.38f, 0.24f);
            Interactable = true;
            Visible = true;
            HorizontalPadding = 8;
        }
#endif
    }
}