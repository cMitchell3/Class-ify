using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 7.0f;
    public float smoothSpeed = 1f;

    // Update camera position
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + new Vector3(0, 0, -distance);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.LookAt(target.position);
        }
    }

    // Set player camera should follow
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
