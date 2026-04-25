using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasFallen = false;

    public float fallGravity = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // يضل ثابت بالبداية
    }

    public void Drop()
    {
        if (hasFallen) return;

        hasFallen = true;
        rb.gravityScale = fallGravity; // يبدأ ينزل
    }
}