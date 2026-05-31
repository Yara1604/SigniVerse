using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PointVis : MonoBehaviour
{
    private Vector3 mOffset = Vector3.zero;
    private bool isDragging = false;
    private Collider2D mCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePosition = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            RaycastHit2D hit = Physics2D.Raycast(worldMousePos, Vector2.zero);

            if (hit.collider != null && hit.collider == mCollider)
            {
                isDragging = true;
                mOffset = transform.position - worldMousePos;
            }
        }

        // Check for dragging
        if(isDragging && mouse.leftButton.isPressed)
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 curScreenPoint = new Vector3(mousePosition.x, mousePosition.y, 0.0f);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;

            // Lock Z axis if you are strictly in 2D space
            curPosition.z = transform.position.z;
            transform.position = curPosition;
        }

        // Check for Release (Equivalent to OnMouseUp)
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        mOffset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector3 curScreenPoint = new Vector3(
              Input.mousePosition.x,
              Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
        transform.position = curPosition;
    }
    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

    }
}
