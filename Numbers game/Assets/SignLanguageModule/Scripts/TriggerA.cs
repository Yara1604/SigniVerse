using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class TriggerA : MonoBehaviour
{
    [Header("Connections")]
    public SignGameManager gameManager;

    [Header("AI Expects")]
    public string AILetter = "?";

    [Header("Puzzle Setup")]
    public string puzzleLetter = "?";

    [Tooltip("The video clip showing how to perform this sign")]
    public VideoClip puzzleVideo; // <-- Added this slot

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pass BOTH the letter and the video to the manager!
            gameManager.OpenPuzzle(AILetter,puzzleLetter, puzzleVideo);

            Destroy(gameObject);
        }
    }
}
