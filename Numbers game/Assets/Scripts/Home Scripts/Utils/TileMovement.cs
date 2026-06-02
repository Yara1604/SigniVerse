using UnityEngine;
using UnityEngine.EventSystems;

public class TileMovement : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public Tile tile { get; set; }
    public JigsawManager manager;

    private Vector3 mOffset = Vector3.zero;
    private SpriteRenderer mSpriteRenderer;

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(tile.xIndex * 100f, tile.yIndex * 100f, 0.0f);
    }

    private void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        if (mSpriteRenderer == null)
        {
            Debug.LogError(
                $"TileMovement on {gameObject.name} is missing a SpriteRenderer!",
                this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (manager != null && !manager.TileMovementEnabled)
            return;

        if (mSpriteRenderer != null)
            mSpriteRenderer.sortingOrder = 100;

        Vector3 screenPos = eventData.position;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        worldPos.z = transform.position.z;

        mOffset = transform.position - worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (manager != null && !manager.TileMovementEnabled)
            return;

        Vector3 screenPos = eventData.position;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));

        worldPos.z = transform.position.z;

        transform.position = worldPos + mOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (manager != null && !manager.TileMovementEnabled)
            return;

        if (mSpriteRenderer != null)
            mSpriteRenderer.sortingOrder = 0;

        float dist = (transform.position - GetCorrectPosition()).magnitude;

        if (dist < 20.0f)
        {
            transform.position = GetCorrectPosition();

            if (manager != null)
            {
                manager.AddCorrectTile();
            }

            Collider2D myCollider = GetComponent<Collider2D>();

            if (myCollider != null)
            {
                myCollider.enabled = false;
            }

            Destroy(this);
        }
    }
}