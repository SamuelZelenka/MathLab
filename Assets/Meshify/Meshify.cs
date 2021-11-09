using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class Meshify : EditorWindow
{
    private enum Direction { North, West, East, South }
    private const float DEFAULT_Y_OFFSET = 200;

    private float xOffset;
    private int scale = 1;
    private Texture2D texture;
    private Color[,] pixelGrid;
    private List<Vector2Int> pixels = new List<Vector2Int>();

    private int triCount;
    private int pixelCount;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Meshify")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        Meshify window = (Meshify)EditorWindow.GetWindow(typeof(Meshify));
        window.Show();
    }

    void OnGUI()
    {
        scale = (int)EditorGUILayout.Slider("Scale", scale, 1, 100);

        texture = (Texture2D)EditorGUILayout.ObjectField("Texture2D", texture, typeof(Texture2D), allowSceneObjects: true);

        if (texture != null)
        {
            if (GUILayout.Button("Calculate Areas"))
            {
                ApplyAreas();
            }
            if (GUILayout.Button("Reset Image"))
            {
                ResetImage();
            }
            DrawImage();
        }
        EditorGUILayout.HelpBox($"{triCount} tris\n {pixelCount} pixel count", MessageType.Info);
    }

    public void ResetImage()
    {
        pixelGrid = new Color[texture.width, texture.height];
        triCount = 0;
        pixelCount = 0;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                pixelGrid[x, y] = texture.GetPixel(x, texture.height - 1 - y);
                if (pixelGrid[x, y].a == 1)
                {
                    pixelCount++;
                }
            }
        }
    }

    public void DrawImage()
    {
        if (texture != null)
        {
            if (pixelGrid == null)
            {
                pixelGrid = new Color[texture.width, texture.height];
            }

            xOffset = position.width / 2 - (texture.width / 2) * scale;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
          
                    Color tempColor = new Color((float)x / (texture.width), (float)y / (texture.height - 1), 0, 1);
                    EditorGUI.DrawRect(new Rect(xOffset + scale * x, DEFAULT_Y_OFFSET + scale * y, scale, scale), pixelGrid[x, y]);
                }
            }
        }
    }

    public void ApplyAreas()
    {
        PixelArea[] areas = GetAreas();


        foreach (PixelArea area in areas)
        {
            Color areaColor = new Color(Random.value, Random.value, Random.value, 1);
            foreach (Vector2Int pixel in area.pixels)
            {
                pixelGrid[pixel.x, pixel.y] = areaColor;
            }
        }
        triCount = areas.Length * 2;
    }

    private PixelArea[] GetAreas()
    {
        List<PixelArea> areas = new List<PixelArea>();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Vector2Int checkPos = new Vector2Int(x, y);

                // If Not transparent calculate area from pixel
                if (pixelGrid[x, y].a == 1)
                {
                    PixelArea newArea = new PixelArea(new Vector2Int[] { });
                    areas.Add(GetArea(new Vector2Int(x, y), GetRange(checkPos), ref newArea));
                }
            }
        }
        return areas.ToArray();
    }

    private PixelArea GetArea(Vector2Int startPos, int range, ref PixelArea pixelArea)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        validPositions.AddRange(pixelArea.pixels);

        for (int y = 0; y < range; y++)
        {
            if (startPos.y + y < pixelGrid.GetLength(1) && pixelGrid[startPos.x, startPos.y + y].a == 1)
            {
                validPositions.Add(new Vector2Int(startPos.x, startPos.y + y));
            }
            else
            {
                return new PixelArea(pixelArea.pixels);
            }
        }
        pixelArea.pixels = validPositions.ToArray();
        foreach (Vector2Int position in pixelArea.pixels)
        {
            pixelGrid[position.x, position.y].a = 0;
        }

        if (startPos.x + 1 < pixelGrid.GetLength(0))
        {
            return GetArea(new Vector2Int(startPos.x + 1, startPos.y), range, ref pixelArea);
        }
        return new PixelArea(pixelArea.pixels);
    }

    private int GetRange(Vector2Int pos)
    {
        int range = 1;

        for (int x = pos.y; x < texture.height - 1; x++)
        {
            if (x + 1 < pixelGrid.GetLength(1) && pixelGrid[pos.x, x + 1].a < 1)
            {
                break;
            }
            range++;
        }
        return range;
    }
}

public struct PixelArea
{
    public Vector2Int[] pixels;
    public PixelArea(Vector2Int[] pixels)
    {
        this.pixels = pixels;
    }
}