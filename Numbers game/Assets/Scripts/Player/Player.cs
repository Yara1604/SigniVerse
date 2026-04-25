using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public FixedJoystick joistick;
    public float moveSpeed = 5f;

    [Header("Visual Rotation")]
    public Transform shipVisual;

    [Header("Game")]
    public int winScore = 2;
    public GameObject winText;

    private int score = 0;

    private void FixedUpdate()
    {
        float hInput = joistick.Horizontal;
        float vInput = joistick.Vertical;

        Vector3 move = new Vector3(hInput, vInput, 0f) * moveSpeed * Time.fixedDeltaTime;
        transform.Translate(move);

        Vector2 moveDirection = new Vector2(hInput, vInput);

        if (moveDirection != Vector2.zero && shipVisual != null)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            shipVisual.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Star"))
        {
            score++;
            Destroy(collision.gameObject);

            if (score >= winScore)
            {
                if (winText != null)
                    winText.SetActive(true);

                SceneManager.LoadScene(4);
            }
        }
    }
}
/*
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public FixedJoystick joistick;
    public float moveSpeed;
    float hInput, vInput;

    int score = 0;
    public int winScore;
    public GameObject winText;
    private void FixedUpdate()
    {
        hInput = joistick.Horizontal * moveSpeed;
        vInput = joistick.Vertical * moveSpeed;
        transform.Translate(hInput, vInput, 0);
    }



    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.tag == "Star")
        {
            score++;
            Destroy(collision.gameObject);
            SceneManager.LoadScene(4);
            if (score >= winScore)
            {
                winText.SetActive(true);
            }

        }
    }
}
*/