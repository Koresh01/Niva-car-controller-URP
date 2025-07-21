using System.Collections;
using UnityEngine;

/// <summary>
/// "�����" ����� ������� ���� �� ������ ����.
/// </summary>
public class PritsepInitializer : MonoBehaviour
{
    public WheelCollider[] wheelColliders;

    IEnumerator Start()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 1f; // "���������"
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 0f;              // ������� ���
            //wheel.brakeTorque = Mathf.Infinity;  // ���������
        }
    }

}
