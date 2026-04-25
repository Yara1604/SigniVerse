using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offsetX = 3f;
    public float fixedY = 0f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offsetX;
        float smoothX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothX, fixedY, -10f);
    }
}
