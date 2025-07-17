using UnityEngine;
using UnityEngine.UI;

public class VelocityListener : MonoBehaviour
{
    public CarInput carInput;

    [SerializeField] Text velocityUI;

    void FixedUpdate()
    {
        velocityUI.text = "Скорость: " + (int) carInput.curSpeed + " км/ч";
    }
}
