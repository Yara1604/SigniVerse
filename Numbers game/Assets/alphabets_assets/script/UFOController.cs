using UnityEngine;
using UnityEngine.InputSystem; // المكتبة الجديدة للتحكم

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 7f;
    public LoseUIManager loseUIManager;

    private Rigidbody2D rb;
    private bool grounded;
    private bool finished = false;
    private bool bridgeStopped = false;
    private bool dead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (finished || bridgeStopped || dead) return;

        // التعديل هنا: استخدام Keyboard.current بدلاً من Input القديم
        bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (GameStart.gameStarted && spacePressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
        }
    }

    void FixedUpdate()
    {
        if (finished || bridgeStopped || dead)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }

        if (GameStart.gameStarted)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (finished || bridgeStopped || dead) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            LoseLevel();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (finished || bridgeStopped || dead) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (finished || bridgeStopped || dead) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    public void FinishLevel()
    {
        finished = true;
        grounded = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void StopAtBridge()
    {
        bridgeStopped = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void ResumeFromBridge()
    {
        bridgeStopped = false;
    }

    public void Jump()
    {
        if (!GameStart.gameStarted)
            GameStart.StartGame();

        if (grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void LoseLevel()
    {
        dead = true;
        grounded = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        GameStart.gameStarted = false;

        if (loseUIManager != null)
        {
            loseUIManager.ShowLosePanel();
        }
    }
}