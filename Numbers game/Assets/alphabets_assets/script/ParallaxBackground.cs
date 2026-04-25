using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;

    float width;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }

    void Update()
    {
        // يحدث العرض إذا تغير الحجم
        width = sr.bounds.size.x;

        if (player.position.x > transform.position.x + width)
        {
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}