using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cameraTransform;
    public float followSpeed = 1f;
    public float fixedY = 0f;
    public float fixedZ = 0f;

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        float newX = cameraTransform.position.x * followSpeed;

        transform.position = new Vector3(newX, fixedY, fixedZ);
    }
}
