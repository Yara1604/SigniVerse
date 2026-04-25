using UnityEngine;

public class PatrolObstacle : MonoBehaviour
{
    public float moveDistance = 3f;   // المسافة من نقطة البداية
    public float moveSpeed = 2f;      // سرعة الحركة

    private Vector3 startPos;
    private bool movingRight = true;
    private SpriteRenderer sr;

    void Start()
    {
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;

            if (transform.position.x >= startPos.x + moveDistance)
            {
                movingRight = false;

                if (sr != null)
                    sr.flipX = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (transform.position.x <= startPos.x - moveDistance)
            {
                movingRight = true;

                if (sr != null)
                    sr.flipX = true;
            }
        }
    }
}