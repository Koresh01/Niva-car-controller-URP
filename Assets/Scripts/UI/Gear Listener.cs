using UnityEngine;
using UnityEngine.UI;

public class GearListener : MonoBehaviour
{
    [SerializeField] CarInput carInput;
    [SerializeField] Text curGearUI;

    void FixedUpdate()
    {
        curGearUI.text = "Передача: " + (carInput.curGearInx-1);
    }
}
