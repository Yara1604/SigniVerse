using UnityEngine;

public class FallingStar : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction = new Vector2(-1, -1);

    void Start()
    {
        speed = Random.Range(3f, 8f);
    }
    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);

        // إذا خرجت من الشاشة
        if (transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }
}