using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class Meshify : EditorWindow
{
    enum Direction { North, West, East, South}
    const float DEFAULT_Y_OFFSET = 200;

    float xOffset;
    float scale = 1;
    float vertexSize = 4;
    Texture2D texture;
    Color[,] pixelGrid;
    List<Vector2Int> pixels = new List<Vector2Int>();
    List<Vector3> vertices = new List<Vector3>();

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
        vertexSize = EditorGUILayout.Slider("Vertex Size",vertexSize, 1, 10);
        scale = EditorGUILayout.Slider("Scale", scale, 1, 100);

        texture = (Texture2D)EditorGUILayout.ObjectField("Texture2D", texture, typeof(Texture2D), allowSceneObjects: true);
        if (GUILayout.Button("Calculate vertices"))
        {
            CalcVertices();
        }
        DrawImage();
    }

    public void DrawImage()
    {
        if (texture != null)
        {
            int index = 0;
            
            pixelGrid = new Color[texture.width, texture.height];
            

            xOffset = position.width / 2 - (texture.width / 2) * scale;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    pixelGrid[x, y] = texture.GetPixel(x, pixelGrid.GetLength(0) - 1 - y);
                    if (pixelGrid[x, y].a == 1)
                    {
                        pixels.Add(new Vector2Int(x, y));
                    }
                    Color tempColor = new Color((float)x/ (texture.width), (float)y/ (texture.height) , 0, 1);
                    EditorGUI.DrawRect(new Rect(xOffset + scale * x, DEFAULT_Y_OFFSET + scale * y, scale, scale), pixelGrid[x, y]);
                    index++;
                }
            }


            foreach (Vector3 vertex in vertices)
            {

                float x = xOffset + scale * vertex.x - vertexSize / 2;
                float y = DEFAULT_Y_OFFSET + scale * vertex.y - vertexSize / 2;

                EditorGUI.DrawRect(new Rect(x , y , vertexSize, vertexSize), Color.red);
            }
        }
    }
    public void CalcVertices()
    {
        

        foreach (Vector2Int pixel in pixels)
        {
            bool[] boolenValues = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                boolenValues[i] = IsPixelInDirection(pixel, (Direction)i);
            }
        }

        bool IsPixelInDirection(Vector2Int pixelPos, Direction direction)
        {
            Vector2Int checkPosition;
            switch (direction)
            {
                case Direction.North:
                    checkPosition = pixelPos + new Vector2Int(0, 1);
                    if (IsInRange(checkPosition.y, 0, pixelGrid.GetLength(1)))
                    {
                        return pixelGrid[checkPosition.x, checkPosition.y].a > 0;
                    }
                    break;
                case Direction.West:
                    checkPosition = pixelPos + new Vector2Int(1, 0);
                    if (IsInRange(pixelPos.x + 1, 0, pixelGrid.GetLength(1)))
                    {
                        return pixelGrid[checkPosition.x, checkPosition.y].a > 0;
                    }
                    break;
                case Direction.East:
                    checkPosition = pixelPos + new Vector2Int(-1, 0);
                    if (IsInRange(pixelPos.x - 1, 0, pixelGrid.GetLength(1)))
                    {
                        return pixelGrid[checkPosition.x, checkPosition.y].a > 0;
                    }
                    break;
                case Direction.South:
                    checkPosition = pixelPos + new Vector2Int(0, -1);
                    if (IsInRange(pixelPos.y - 1, 0, pixelGrid.GetLength(1)))
                    {
                        return pixelGrid[checkPosition.x, checkPosition.y].a > 0;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

    }
    public static bool IsInRange(float input, float min, float max)
    {
        return input >= min && input <= max;
    }

    private void AddVertex(Vector3 vertex)
    {
        if (!vertices.Contains(vertex))
        {
            vertices.Add(vertex);
        }
    }
}