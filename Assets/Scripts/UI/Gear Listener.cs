using UnityEngine;
using UnityEngine.UI;

public class GearListener : MonoBehaviour
{
    [SerializeField] CarInput carInput;
    [SerializeField] Text curGearUI;

    void FixedUpdate()
    {
        int RealGearInx = carInput.curGearInx - 1;

        if (RealGearInx == -1)
            curGearUI.text = "R";
        else if (RealGearInx == 0)
            curGearUI.text = "N";
        else
            curGearUI.text = RealGearInx.ToString();
    }
}
