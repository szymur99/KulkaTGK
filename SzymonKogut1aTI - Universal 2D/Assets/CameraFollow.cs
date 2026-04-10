using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Drag your ball here
    public Vector3 offset = new Vector3(0, 0, -10);  // Camera offset
    public float smoothSpeed = 0.125f;  // Smoothing factor (0 = no smoothing, 1 = instant)

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Desired position = ball position + offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply to camera
        transform.position = smoothedPosition;
    }
}