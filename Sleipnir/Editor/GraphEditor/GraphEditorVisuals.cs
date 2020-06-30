using UnityEngine;

namespace Sleipnir.Editor
{
    public partial class GraphEditor
    {
        private static readonly Color GridLineColor = new Color(0.28f, 0.28f, 0.28f);
        private static readonly Color CrossColor = new Color(0.36f, 0.36f, 0.36f);
        private static readonly Color GridBackgroundColor = new Color(0.2f, 0.2f, 0.2f);

        private static Texture2D _gridTexture;
        private static Texture2D _crossTexture;

        private static void LoadTextures()
        {
            _gridTexture = GenerateGridTexture(GridLineColor, GridBackgroundColor);
            _crossTexture = GenerateCrossTexture(CrossColor);
        }

        private static Texture2D GenerateGridTexture(Color line, Color background)
        {
            var texture = new Texture2D(64, 64);
            var colors = new Color[64 * 64];
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var col = background;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, background, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, background, 0.35f);
                    colors[y * 64 + x] = col;
                }
            }
            texture.SetPixels(colors);
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;
            texture.name = "Grid";
            texture.Apply();
            return texture;
        }

        private static Texture2D GenerateCrossTexture(Color line)
        {
            var texture = new Texture2D(64, 64);
            var colors = new Color[64 * 64];
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    colors[y * 64 + x] = col;
                }
            }
            texture.SetPixels(colors);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.name = "Cross";
            texture.Apply();
            return texture;
        }
    }
}