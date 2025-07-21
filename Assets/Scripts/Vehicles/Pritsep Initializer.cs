using System.Collections;
using UnityEngine;

/// <summary>
/// "Будит" колёса прицепа чтоб на старте игры.
/// </summary>
public class PritsepInitializer : MonoBehaviour
{
    public WheelCollider[] wheelColliders;

    IEnumerator Start()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 1f; // "подтолкнём"
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 0f;              // убираем газ
            //wheel.brakeTorque = Mathf.Infinity;  // фиксируем
        }
    }

}
