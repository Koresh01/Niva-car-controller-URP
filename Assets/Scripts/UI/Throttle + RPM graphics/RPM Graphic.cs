using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� ������ ������ ������� (throttle) �� UI-�������, ��������� ������������� ���������� ��������� ��������.
/// </summary>
public class RPMGraphic : MonoBehaviour
{
    [Header("������:")]
    [Tooltip("������, ���������� �������� ������ ����.")]
    public CarInput carInput;

    [Header("��������� �������:")]
    [Tooltip("UI Image, �� ������� ������������ ������.")]
    public Image graphImage;

    [Tooltip("������� ��������� �������� ���������� �� �������.")]
    public int historyLength = 100;

    [Header("������� ���������� (���):")]
    private float redrawInterval = 0.1f; // ��� � 0.1 ��� (10 FPS)
    private float redrawTimer = 0f;

    private List<Vector2> points = new();
    private GraphPlot plot;

    void Start()
    {
        plot = new GraphPlot(graphImage, 256, historyLength);
        plot.SetFixedRange(0, historyLength, -0.1f, 1.1f); // X: �� 0 �� N-1, Y: �� 0 �� 1 (��� throttle)
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
            ShiftLeft(); // ������� ��� �������� �����, ����� ���������� ����� ��� ������
            points[^1] = new Vector2(historyLength - 1, currentThrottle); // ��������� X � �������������
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
    /// �������� ��� ����� �� ���� ������� �����, ������ ����� ������.
    /// </summary>
    private void ShiftLeft()
    {
        for (int i = 1; i < historyLength; i++)
        {
            points[i - 1] = new Vector2(i - 1, points[i].y);
        }
    }
}
