using UnityEngine;
using UnityEngine.UIElements;

public class Camera : MonoBehaviour
{
    public Transform Target;

    public float anlge = 0;
    public Vector3 offset;
    public Vector3 targetPos;

    private void Start()
    {
        targetPos = Target.position;
    }

    private void FixedUpdate()
    {
        anlge += 1f/180f;
        
        // Позиция камеры
        Vector3 nextCamPosition = new Vector3(targetPos.x + offset.x * Mathf.Cos(anlge), transform.position.y, targetPos.z + offset.z * Mathf.Sin(anlge));
        transform.position = nextCamPosition;
        
        // Поворот камеры:
        Vector3 dir = targetPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

    }
}
