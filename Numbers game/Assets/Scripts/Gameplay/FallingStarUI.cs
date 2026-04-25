using UnityEngine;

public class FallingStarUI : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction = new Vector2(-1, -1);

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        speed = Random.Range(3f, 8f);
    }

    void Update()
    {
        // الحركة باستخدام UI
        rectTransform.anchoredPosition += direction.normalized * speed * 100 * Time.deltaTime;

        // إذا خرجت من الشاشة
        if (rectTransform.anchoredPosition.y < -600)
        {
            Destroy(gameObject);
        }
    }
}