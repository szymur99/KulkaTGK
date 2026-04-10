using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;   // Drag the ball here

    void LateUpdate()
    {
        if (target == null) return;

        // Make the camera's position equal to the ball's position, but keep its own Z
        Vector3 newPos = target.transform.position;
        newPos.z = transform.position.z;   // Preserve camera's original Z (usually -10)
        transform.position = newPos;
    }
}