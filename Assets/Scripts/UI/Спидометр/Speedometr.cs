using UnityEngine;

public class Speedometr : MonoBehaviour
{
    [Header("������")]
    public CarInput carInput;           // ������, ������ ������� curRPM
    public RectTransform needle;        // ������ �� ������� ��������� (Image -> RectTransform)

    [Header("�������� ����������� �������")]
    public float needleSpeed = 1.0f;

    [Header("������������ ��������")]
    public float maxSpeed = 180;        // ������������ �������

    [Header("��������� �������")]
    [Tooltip("���� �������� ������� ��� ����������� �������� (������ �����)")]
    public float minAngle = -120f;

    [Tooltip("���� �������� ������� ��� ������������ �������� (������ ������)")]
    public float maxAngle = 120f;

    void Update()
    {
        // ��������� �������� � ��������������� �� 0 �� 1.
        float normalizedSpeed = carInput.curSpeed / maxSpeed;

        // ������������� ���� �������� �������
        float angle = Mathf.Lerp(minAngle, maxAngle, normalizedSpeed);

        // ��������� �������� � �������
        Quaternion targetNeedleRotation = Quaternion.Euler(0, 0, -angle);
        needle.localRotation = Quaternion.Lerp(needle.localRotation, targetNeedleRotation, needleSpeed * Time.deltaTime);

    }
}
