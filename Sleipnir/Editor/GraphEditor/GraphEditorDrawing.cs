using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        protected override void DrawEditor(int index)
        {
            if (_graph == null)
                return;
            
            GUIHelper.PushMatrix(GUI.matrix);
            ProcessInput();
            DrawGrid();
            BeginZoomed();
            DrawConnectionToMouse();
            base.DrawEditor(index);
            EndZoomed();
            GUIHelper.PopMatrix();
            
            EditorUtility.SetDirty(_serializationRoot);
        }
        
        private void DrawGrid()
        {
            if (_gridTexture == null || _crossTexture == null)
                return;

            var windowRect = new Rect(Vector2.zero, position.size);
            var center = windowRect.size * 0.5f;

            // Offset from origin in tile units
            var xOffset = -(center.x * Zoom + Pan.x)
                / _gridTexture.width;
            var yOffset = ((center.y - windowRect.size.y) * Zoom + Pan.y)
                / _gridTexture.height;

            var tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            var tileAmountX = Mathf.Round(windowRect.size.x * Zoom)
                / _gridTexture.width;
            var tileAmountY = Mathf.Round(windowRect.size.y * Zoom)
                / _gridTexture.height;

            var tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(windowRect, _gridTexture,
                new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(windowRect, _crossTexture,
                new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }

        private void DrawConnectionToMouse()
        {
            if (_selectedSlot == null)
                return;

            var slotCenter = _selectedSlot.GridRect.center;
            var mousePosition = GuiToGridPosition(Event.current.mousePosition / Zoom);

            if (_selectedSlot.Direction == SlotDirection.Output)
                ConnectionExtensions.DrawConnection(slotCenter, mousePosition, this, _selectedSlot.Color);
            else
                ConnectionExtensions.DrawConnection(mousePosition, slotCenter, this, _selectedSlot.Color);
        }
    }
}