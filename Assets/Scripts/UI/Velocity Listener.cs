using UnityEngine;
using UnityEngine.UI;

public class VelocityListener : MonoBehaviour
{
    [SerializeField] Rigidbody carPhysics;
    [SerializeField] Text velocityUI;

    void FixedUpdate()
    {
        velocityUI.text = "Скорость: " + (carPhysics.linearVelocity.magnitude * 3.6f).ToString("F1") + " км/ч";
    }
}
