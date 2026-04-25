using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    public FallingObstacle fallingObstacle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fallingObstacle.Drop();
        }
    }
}