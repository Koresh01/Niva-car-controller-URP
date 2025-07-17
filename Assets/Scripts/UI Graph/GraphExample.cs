using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображает график подачи топлива (throttle) на UI-графике, используя фиксированное количество последних значений.
/// </summary>
public class GraphExample : MonoBehaviour
{
    [Header("Ссылки:")]
    [Tooltip("Скрипт, содержащий значение педали газа.")]
    public CarInput carInput;

    [Header("Настройки графика:")]
    [Tooltip("UI Image, на котором отображается график.")]
    public Image graphImage;

    [Tooltip("Сколько последних значений отображать на графике.")]
    public int historyLength = 100;

    private List<Vector2> points = new();
    private GraphPlot plot;

    void Start()
    {
        plot = new GraphPlot(graphImage, 256, 256);
        plot.SetFixedRange(0, historyLength - 1, -0.1f, 1.1f); // X: от 0 до N-1, Y: от 0 до 1 (для throttle)
    }

    void FixedUpdate()
    {
        float currentThrottle = carInput.throttleInput;

        if (points.Count < historyLength)
        {
            points.Add(new Vector2(points.Count, currentThrottle));
        }
        else
        {
            ShiftLeft(); // Смещаем все значения влево, чтобы освободить место для нового
            points[^1] = new Vector2(historyLength - 1, currentThrottle); // Последний X — фиксированный
        }

        plot.SetPoints(points);
        plot.Redraw();
    }

    /// <summary>
    /// Сдвигает все точки на одну позицию влево, удаляя самую старую.
    /// </summary>
    private void ShiftLeft()
    {
        for (int i = 1; i < historyLength; i++)
        {
            points[i - 1] = new Vector2(i - 1, points[i].y);
        }
    }
}
