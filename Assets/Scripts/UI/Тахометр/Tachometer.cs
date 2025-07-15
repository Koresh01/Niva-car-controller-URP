using UnityEngine;
using UnityEngine.UI;

public class Tachometer : MonoBehaviour
{
    [Header("������")]
    public CarInput carInput;           // ������, ������ ������� curRPM
    public RectTransform needle;        // ������ �� ������� ��������� (Image -> RectTransform)

    [Header("�������� ����������� �������")]
    public float needleSpeed = 1.0f;

    [Header("��������� ���������")]
    public float minRPM = 800f;         // �������� ���
    public float maxRPM = 7000f;        // ������������ �������

    [Header("��������� �������")]
    [Tooltip("���� �������� ������� ��� ����������� �������� (������ �����)")]
    public float minAngle = -120f;

    [Tooltip("���� �������� ������� ��� ������������ �������� (������ ������)")]
    public float maxAngle = 120f;

    void Update()
    {
        // ��������� ��������������� curRPM � �������� RPM
        float rpm = Mathf.Lerp(minRPM, maxRPM, Mathf.Clamp01(carInput.curRPM));

        // ������������� ���� �������� ������� �� ��������� RPM
        float rpm01 = (rpm - minRPM) / (maxRPM - minRPM); // �����������
        float angle = Mathf.Lerp(minAngle, maxAngle, rpm01);

        // ��������� �������� � �������
        Quaternion targetNeedleRotation = Quaternion.Euler(0, 0, -angle);
        needle.localRotation = Quaternion.Lerp(needle.localRotation, targetNeedleRotation, needleSpeed * Time.deltaTime);

    }
}
