using UnityEngine;

public class FlyingObstacle : MonoBehaviour
{
    public float speed = 6f;
    private bool isFlying = false;

    public void StartFlying(Transform player)
    {
        if (isFlying) return;
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying) return;

        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}