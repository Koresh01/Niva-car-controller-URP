using UnityEngine;

public class CenterOfMassSetter : MonoBehaviour
{
    [SerializeField] private Transform centerOfMassTransform;

    void Start()
    {
        if (centerOfMassTransform != null)
        {
            GetComponent<Rigidbody>().centerOfMass =
                transform.InverseTransformPoint(centerOfMassTransform.position);
        }
    }

    void OnDrawGizmos()
    {
        if (centerOfMassTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(centerOfMassTransform.position, 0.1f);
        }
    }
}
