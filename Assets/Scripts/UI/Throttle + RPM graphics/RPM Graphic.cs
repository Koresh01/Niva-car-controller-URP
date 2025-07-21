using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображает график подачи топлива (throttle) на UI-графике, используя фиксированное количество последних значений.
/// </summary>
public class RPMGraphic : MonoBehaviour
{
    [Header("Ссылки:")]
    [Tooltip("Скрипт, содержащий значение педали газа.")]
    public CarInput carInput;

    [Header("Настройки графика:")]
    [Tooltip("UI Image, на котором отображается график.")]
    public Image graphImage;

    [Tooltip("Сколько последних значений отображать на графике.")]
    public int historyLength = 100;

    [Header("Частота обновления (сек):")]
    private float redrawInterval = 0.1f; // раз в 0.1 сек (10 FPS)
    private float redrawTimer = 0f;

    private List<Vector2> points = new();
    private GraphPlot plot;

    void Start()
    {
        plot = new GraphPlot(graphImage, 256, historyLength);
        plot.SetFixedRange(0, historyLength, -0.1f, 1.1f); // X: от 0 до N-1, Y: от 0 до 1 (для throttle)
        plot.lineColor = Color.red;
    }

    void FixedUpdate()
    {
        float currentThrottle = Mathf.Clamp01(carInput.curRPM);

        if (points.Count < historyLength)
        {
            points.Add(new Vector2(points.Count, currentThrottle));
        }
        else
        {
            ShiftLeft(); // Смещаем все значения влево, чтобы освободить место для нового
            points[^1] = new Vector2(historyLength - 1, currentThrottle); // Последний X — фиксированный
        }

        redrawTimer += Time.fixedDeltaTime;
        if (redrawTimer >= redrawInterval)
        {
            plot.SetPoints(points);
            plot.Redraw();
            redrawTimer = 0f;
        }
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
