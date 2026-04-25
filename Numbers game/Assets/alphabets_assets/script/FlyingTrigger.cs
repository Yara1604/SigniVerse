using UnityEngine;

public class FlyingTrigger : MonoBehaviour
{
    public FlyingObstacle flyingObstacle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flyingObstacle.StartFlying(collision.transform);
        }
    }
}