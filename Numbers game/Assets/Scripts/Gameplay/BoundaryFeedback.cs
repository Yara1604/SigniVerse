using UnityEngine;

public partial class BoundaryFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color alertColor = Color.red;
    public Color normalColor = new Color(1, 1, 1, 0); // شفاف تماماً

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // التأكد من أن اللون يبدأ شفافاً
        spriteRenderer.color = normalColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) spriteRenderer.color = alertColor;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) spriteRenderer.color = normalColor;
    }
}