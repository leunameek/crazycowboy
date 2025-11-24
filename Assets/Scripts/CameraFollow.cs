using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("A quien persigo?")]
    public Transform target;
    [Header("Suavidad")]
    public float smoothTime = 0.25f;
    [Header("Distancia de seguridad")]
    public Vector3 offset;
    
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
