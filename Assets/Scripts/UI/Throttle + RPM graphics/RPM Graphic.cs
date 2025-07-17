using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ќтображает график подачи топлива (throttle) на UI-графике, использу€ фиксированное количество последних значений.
/// </summary>
public class RPMGraphic : MonoBehaviour
{
    [Header("—сылки:")]
    [Tooltip("—крипт, содержащий значение педали газа.")]
    public CarInput carInput;

    [Header("Ќастройки графика:")]
    [Tooltip("UI Image, на котором отображаетс€ график.")]
    public Image graphImage;

    [Tooltip("—колько последних значений отображать на графике.")]
    public int historyLength = 100;

    private List<Vector2> points = new();
    private GraphPlot plot;

    void Start()
    {
        plot = new GraphPlot(graphImage, 128, historyLength);
        plot.SetFixedRange(0, historyLength - 1, -0.1f, 1.1f); // X: от 0 до N-1, Y: от 0 до 1 (дл€ throttle)
        plot.lineColor = Color.red;
    }

    void FixedUpdate()
    {
        float currentThrottle = carInput.curRPM;

        if (points.Count < historyLength)
        {
            points.Add(new Vector2(points.Count, currentThrottle));
        }
        else
        {
            ShiftLeft(); // —мещаем все значени€ влево, чтобы освободить место дл€ нового
            points[^1] = new Vector2(historyLength - 1, currentThrottle); // ѕоследний X Ч фиксированный
        }

        plot.SetPoints(points);
        plot.Redraw();
    }

    /// <summary>
    /// —двигает все точки на одну позицию влево, удал€€ самую старую.
    /// </summary>
    private void ShiftLeft()
    {
        for (int i = 1; i < historyLength; i++)
        {
            points[i - 1] = new Vector2(i - 1, points[i].y);
        }
    }
}
