using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Класс, отвечающий за отрисовку графика на текстуре, которую можно прикрепить к UI Image.
/// Позволяет рисовать линии между точками и очищать изображение.
/// </summary>
public class GraphRenderer
{
    /// <summary>
    /// Ширина текстуры в пикселях.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Высота текстуры в пикселях.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Текстура, на которой осуществляется рисование.
    /// </summary>
    private Texture2D texture;

    /// <summary>
    /// UI Image, в которую встроена текстура.
    /// </summary>
    private Image targetImage;

    /// <summary>
    /// Конструктор, создающий текстуру и прикрепляющий её к UI Image.
    /// </summary>
    /// <param name="imageTarget">Целевой UI Image для отображения графика.</param>
    /// <param name="width">Ширина текстуры (по умолчанию 512).</param>
    /// <param name="height">Высота текстуры (по умолчанию 512).</param>
    public GraphRenderer(Image imageTarget, int width = 512, int height = 512)
    {
        this.targetImage = imageTarget;
        this.Width = width;
        this.Height = height;

        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero);
        Clear();
    }

    /// <summary>
    /// Очищает текстуру, заливая её указанным цветом (по умолчанию чёрным).
    /// </summary>
    /// <param name="color">Цвет заливки. Если null, используется чёрный.</param>
    public void Clear(Color? color = null)
    {
        Color c = color ?? new Color(0, 0, 0, 0); // прозрачный по умолчанию
        Color[] pixels = new Color[Width * Height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = c;
        texture.SetPixels(pixels);
        texture.Apply();
    }

    /// <summary>
    /// Рисует линии между заданными точками на текстуре.
    /// </summary>
    /// <param name="points">Список точек (в пикселях), соединяемых линиями.</param>
    /// <param name="color">Цвет линий.</param>
    /// <param name="thickness">Толщина линий (не используется в текущей реализации).</param>
    public void DrawGraph(List<Vector2> points, Color color, float thickness = 1f)
    {
        int pixelThickness = Mathf.RoundToInt(thickness);

        for (int i = 1; i < points.Count; i++)
        {
            DrawLine(points[i - 1], points[i], color, pixelThickness);
        }

        texture.Apply();
    }

    /// <summary>
    /// Рисует оси X и Y по данным точек (находится 0 по X и Y).
    /// </summary>
    /// <param name="data">Оригинальные точки графика (ненормализованные).</param>
    /// <param name="color">Цвет осей.</param>
    public void DrawAxes(List<Vector2> data, Color color)
    {
        if (data == null || data.Count == 0) return;

        float xMin = data.Min(p => p.x);
        float xMax = data.Max(p => p.x);
        float yMin = data.Min(p => p.y);
        float yMax = data.Max(p => p.y);

        // Где находится (0,0) на текстуре
        float zeroX = Mathf.InverseLerp(xMin, xMax, 0f) * Width;
        float zeroY = Mathf.InverseLerp(yMin, yMax, 0f) * Height;

        // Если 0 вне диапазона, то оси не рисуем
        bool drawX = zeroY >= 0 && zeroY <= Height;
        bool drawY = zeroX >= 0 && zeroX <= Width;

        if (drawX)
            DrawLine(new Vector2(0, zeroY), new Vector2(Width, zeroY), color);
        if (drawY)
            DrawLine(new Vector2(zeroX, 0), new Vector2(zeroX, Height), color);
    }

    /// <summary>
    /// Рисует линию между двумя точками на текстуре с помощью алгоритма Брезенхэма.
    /// </summary>
    /// <param name="p1">Начальная точка линии.</param>
    /// <param name="p2">Конечная точка линии.</param>
    /// <param name="color">Цвет линии.</param>
    private void DrawLine(Vector2 p1, Vector2 p2, Color color, int thickness = 1)
    {
        int x0 = Mathf.RoundToInt(p1.x);
        int y0 = Mathf.RoundToInt(p1.y);
        int x1 = Mathf.RoundToInt(p2.x);
        int y1 = Mathf.RoundToInt(p2.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            SetPixelThick(x0, y0, color, thickness);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
    private void SetPixelThick(int x, int y, Color color, int thickness)
    {
        int half = thickness / 2;
        for (int dx = -half; dx <= half; dx++)
        {
            for (int dy = -half; dy <= half; dy++)
            {
                SetPixelSafe(x + dx, y + dy, color);
            }
        }
    }


    /// <summary>
    /// Устанавливает цвет пикселя в пределах границ текстуры.
    /// </summary>
    /// <param name="x">X-координата пикселя.</param>
    /// <param name="y">Y-координата пикселя.</param>
    /// <param name="color">Цвет пикселя.</param>
    private void SetPixelSafe(int x, int y, Color color)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            texture.SetPixel(x, y, color);
    }
}
