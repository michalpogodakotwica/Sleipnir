using System;
using UnityEditor;
using UnityEngine;

namespace Sleipnir.Editor
{
    public static class ConnectionExtensions
    {
        private const float ConnectionWidth = 5f;

        private static Tuple<Vector2, Vector2, Vector2, Vector2> BezierCurveData
            (Vector2 startGridPosition, Vector2 endGridPosition)
        {
            var distance = endGridPosition - startGridPosition;
            var angle = Mathf.Atan2(distance.y, distance.x) / 4;
            var outputVector = new Vector2(1, Mathf.Sin(angle));
            var inputVector = Mathf.Abs(Mathf.Atan2(distance.y, distance.x)) * Mathf.Rad2Deg > 150
                ? new Vector2(-1, Mathf.Sin(angle))
                : new Vector2(-1, -Mathf.Sin(angle));
            var multiplier = (endGridPosition - startGridPosition).magnitude * 0.95f;

            return new Tuple<Vector2, Vector2, Vector2, Vector2>(
                startGridPosition,
                endGridPosition,
                startGridPosition + outputVector * multiplier,
                endGridPosition + inputVector * multiplier);
        }

        internal static void DrawConnection(Vector2 startGridPosition, Vector2 endGridPosition,
            GraphEditor editor, Color color)
        {
            if(startGridPosition == endGridPosition)
                return;

            var bezierCurveData = BezierCurveData(startGridPosition, endGridPosition);

            Handles.DrawBezier(
                editor.GridToGuiPositionNoClip(bezierCurveData.Item1),
                editor.GridToGuiPositionNoClip(bezierCurveData.Item2),
                editor.GridToGuiPositionNoClip(bezierCurveData.Item3),
                editor.GridToGuiPositionNoClip(bezierCurveData.Item4),
                color,
                null,
                ConnectionWidth);
        }

        internal static void Draw(this Connection connection, GraphEditor editor)
        {
            var outputSlot = connection.OutputSlot;
            var inputSlot = connection.InputSlot;

            var startGridPosition = outputSlot.GridRect.center;
            var endGridPosition = inputSlot.GridRect.center;

            if (startGridPosition == endGridPosition)
                return;
            
            var color = outputSlot.Color == inputSlot.Color
                ? outputSlot.Color
                : Color.Lerp(inputSlot.Color, outputSlot.Color, 0.5f);

            DrawConnection(startGridPosition, endGridPosition, editor, color);
        }

        internal static bool IsPointOverConnection(this Connection connection, Vector2 gridPoint)
        {
            var startGridPosition = connection.OutputSlot.GridRect.center;
            var endGridPosition = connection.InputSlot.GridRect.center;

            if (startGridPosition == endGridPosition)
                return false;
            
            var bezierCurveData = BezierCurveData(startGridPosition, endGridPosition);
            return HandleUtility.DistancePointBezier(
                       gridPoint,
                       bezierCurveData.Item1,
                       bezierCurveData.Item2,
                       bezierCurveData.Item3,
                       bezierCurveData.Item4) < ConnectionWidth;
        }
    }
}