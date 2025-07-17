using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


/// <summary>
/// Класс для хранения и управления точками графика, а также для их отрисовки на UI-изображении через GraphRenderer.
/// </summary>
public class GraphPlot
{
    private float? fixedXMin = null, fixedXMax = null;
    private float? fixedYMin = null, fixedYMax = null;

    [Tooltip("Цвет линии графика.")]
    public Color lineColor;

    /// <summary>
    /// Список всех добавленных точек графика (в пользовательских координатах).
    /// </summary>
    private List<Vector2> points = new();

    /// <summary>
    /// Объект, отвечающий за отрисовку графика на текстуре.
    /// </summary>
    private GraphRenderer renderer;

    /// <summary>
    /// Создаёт новый экземпляр графика, автоматически инициализируя отрисовщик с заданной текстурой.
    /// </summary>
    /// <param name="imageTarget">UI Image, в которую будет отрисован график.</param>
    /// <param name="width">
    /// Ширина текстуры в пикселях (по умолчанию 512).
    /// От разрешения зависит качество отрисовки: чем выше значение, тем чётче линии и меньше пикселизация,
    /// особенно при масштабировании UI.
    /// Однако более высокое разрешение увеличивает нагрузку на память и производительность,
    /// особенно при частом обновлении графика.
    /// </param>
    /// <param name="height">
    /// Высота текстуры в пикселях (по умолчанию 512).
    /// Влияет аналогично ширине: выше качество, но выше нагрузка.
    /// Рекомендуется использовать разрешение 256–512 для балансировки качества и производительности.
    /// </param>
    public GraphPlot(Image imageTarget, int width = 512, int height = 512)
    {
        renderer = new GraphRenderer(imageTarget, width, height);
    }

    /// <summary>
    /// Устанавливает пользовательский набор точек.
    /// </summary>
    public void SetPoints(List<Vector2> points)
    {
        this.points = points;
    }
    /// <summary>
    /// Добавляет точку на график и перерисовывает его.
    /// </summary>
    /// <param name="x">Значение по оси X.</param>
    /// <param name="y">Значение по оси Y.</param>
    public void AddPoint(float x, float y)
    {
        points.Add(new Vector2(x, y));
        Redraw();
    }

    /// <summary>
    /// Удаляет последнюю добавленную точку с графика и перерисовывает его.
    /// </summary>
    public void RemoveLast()
    {
        if (points.Count > 0)
        {
            points.RemoveAt(points.Count - 1);
            Redraw();
        }
    }

    /// <summary>
    /// Очищает все точки и очищает изображение графика.
    /// </summary>
    public void Clear()
    {
        points.Clear();
        renderer.Clear();
    }

    /// <summary>
    /// Перерисовывает график на основе текущих точек. Если точек меньше двух — просто очищает график.
    /// </summary>
    public void Redraw()
    {
        if (points.Count < 2)
        {
            renderer.Clear();
            return;
        }

        var pixelPoints = NormalizePoints(points, renderer.Width, renderer.Height);
        renderer.Clear();
        renderer.DrawAxes(points, Color.gray); // ⬅️ Новый вызов для отрисовки осей
        renderer.DrawGraph(pixelPoints, lineColor, thickness: 2f);
    }

    public void SetFixedRange(float? xMin, float? xMax, float? yMin, float? yMax)
    {
        fixedXMin = xMin;
        fixedXMax = xMax;
        fixedYMin = yMin;
        fixedYMax = yMax;
    }
    /// <summary>
    /// Преобразует список пользовательских координат в координаты пикселей для отрисовки на текстуре.
    /// </summary>
    /// <param name="data">Список точек в пользовательских координатах.</param>
    /// <param name="width">Ширина текстуры в пикселях.</param>
    /// <param name="height">Высота текстуры в пикселях.</param>
    /// <returns>Список точек в пиксельных координатах.</returns>
    private List<Vector2> NormalizePoints(List<Vector2> data, int width, int height)
    {
        float xMin = fixedXMin ?? data.Min(p => p.x);
        float xMax = fixedXMax ?? data.Max(p => p.x);
        float yMin = fixedYMin ?? data.Min(p => p.y);
        float yMax = fixedYMax ?? data.Max(p => p.y);

        return data.Select(p =>
        {
            float x = Mathf.InverseLerp(xMin, xMax, p.x) * width;
            float y = Mathf.InverseLerp(yMin, yMax, p.y) * height;
            return new Vector2(x, y);
        }).ToList();
    }

}
