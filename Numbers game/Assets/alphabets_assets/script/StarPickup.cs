using UnityEngine;

public class StarPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelStarsManager.Instance != null)
            {
                LevelStarsManager.Instance.AddStar();
            }

            Destroy(gameObject);
        }
    }
}