using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public Transform otherGround;
    private float width;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (transform.position.x + width < Camera.main.transform.position.x)
        {
            transform.position = new Vector3(
                otherGround.position.x + width,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
