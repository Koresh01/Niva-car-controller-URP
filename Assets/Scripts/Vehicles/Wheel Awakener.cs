using UnityEngine;

public class WheelAwakener : MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    public float minMotorTorque = 0.01f;
    public float minRPMThreshold = 1f;

    private void Start()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = 1f;
        }
    }

    /*
    private void FixedUpdate()
    {
        foreach (var wheel in wheelColliders)
        {
            if (Mathf.Abs(wheel.rpm) < minRPMThreshold)
            {
                // Немного "подтолкнём", чтобы физика заработала
                wheel.motorTorque = minMotorTorque;
            }
            else
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = 0f;
            }
        }
    }
    */
}
