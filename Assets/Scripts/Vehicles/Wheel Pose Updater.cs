using UnityEngine;

public class WheelPoseUpdater : MonoBehaviour
{
    public Wheel[] wheels;

    void Update()
    {
        foreach (var wheel in wheels)
        {
            Vector3 pos;
            Quaternion rot;
            wheel.collider.GetWorldPose(out pos, out rot);
            wheel.visual.position = pos;
            wheel.visual.rotation = rot;
        }
    }
}
