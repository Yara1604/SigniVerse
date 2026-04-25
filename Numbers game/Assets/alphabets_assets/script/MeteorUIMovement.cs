using UnityEngine;

public class MeteorUIMovement : MonoBehaviour
{
    public float moveX = 220f;   // لليمين
    public float moveY = -160f;  // لتحت

    public float destroyX = 800f;
    public float destroyY = -500f;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.anchoredPosition += new Vector2(moveX, moveY) * Time.deltaTime;

        if (rt.anchoredPosition.x > destroyX || rt.anchoredPosition.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}