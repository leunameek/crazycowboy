using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.25f;
    public Vector3 offset;
    
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target != null)
        {
            // Calculate initial offset based on current positions
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
