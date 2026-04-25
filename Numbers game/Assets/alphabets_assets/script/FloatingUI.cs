using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatAmount = 10f;
    public float offset = 0f;

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed + offset) * floatAmount;
        rectTransform.anchoredPosition = new Vector2(startPos.x, newY);
    }
}