using UnityEngine;

/// <summary>
/// Скрипт следования камеры за автомобилем.
/// </summary>
public class Camera : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform car;
    [SerializeField] Vector3 offset = new Vector3(0,3,-4);

    private void FixedUpdate()
    {
        MoveHandler();
        RotationHandler();
    }

    void MoveHandler()
    {
        Vector3 targetPos = car.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

    void RotationHandler()
    {
        var direction = car.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
    }


}
