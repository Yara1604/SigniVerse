using UnityEngine;

public class TileMovement : MonoBehaviour
{
    public Tile tile { get; set; }
    public JigsawManager manager; // Make sure its assigned when tile created

    private Vector3 mOffset = Vector3.zero;
    private SpriteRenderer mSpriteRenderer;

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(tile.xIndex * 100f, tile.yIndex * 100f, 0.0f);
    }

    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();

        if (mSpriteRenderer == null)
        {
            Debug.LogError($"TileMovement on {gameObject.name} is missing a SpriteRenderer!", this);
        }
    }

    private void OnMouseDown()
    {
        // SAFEGUARD: Only allow movement if the manager says it is okay
        if (manager != null && !manager.TileMovementEnabled) return;

        // Bring the piece to the front layer so it doesn't slide under others
        if (mSpriteRenderer != null) mSpriteRenderer.sortingOrder = 100;

        // Calculate the offset so the tile doesn't snap its center exactly to the mouse
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        mOffset = transform.position - mouseWorldPos;
        mOffset.z = 0; // Lock the Z axis


    }

    private void OnMouseDrag()
    {
        if (manager != null && !manager.TileMovementEnabled) return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;

        // Keep the Z position exactly where it started
        curPosition.z = transform.position.z;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        if (manager != null && !manager.TileMovementEnabled) return;

        // Drop the piece back down to the normal layer
        if (mSpriteRenderer != null) mSpriteRenderer.sortingOrder = 0;

        // Check how close we are to the correct spot
        float dist = (transform.position - GetCorrectPosition()).magnitude;

        if (dist < 20.0f)
        {
            // Snap into place
            transform.position = GetCorrectPosition();

            // Tell the manager we scored
            if (manager != null)
            {
                manager.AddCorrectTile();
            }

            // Turn off the collider so the mouse can never click this piece again
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }

            // Destroy this movement script entirely to free up memory
            Destroy(this);
        }
    }
}