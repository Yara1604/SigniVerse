using UnityEngine;
using UnityEngine.Video; // <-- Added this so it knows what a VideoPlayer is
using TMPro;

public class SignGameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiVisuals;
    public TextMeshProUGUI targetPromptText;
    public SignFeedbackManager feedbackManager;
    public GameObject continueButton;

    [Tooltip("Drag your UI Video Player here")]
    public VideoPlayer referenceVideoPlayer; // <-- Added this slot

    void Start()
    {
        uiVisuals.SetActive(false);
    }

    public void OpenPuzzle(string aiTarget, string displayText, VideoClip signVideo)
    {
        uiVisuals.SetActive(true);
        if (continueButton != null) continueButton.SetActive(false);

        // 1. Update the UI text for the human to read
        targetPromptText.text = displayText;
        targetPromptText.color = Color.black;

        // 2. Pass the strict target to the AI Feedback Manager
        feedbackManager.SetupNextSign(aiTarget);

        // 3. Play the video
        if (referenceVideoPlayer != null && signVideo != null)
        {
            referenceVideoPlayer.clip = signVideo;
            referenceVideoPlayer.Play();
        }
    }

    public void OnSignSuccess()
    {
        //targetPromptText.text = "?????!";
        targetPromptText.color = Color.green;
        if (continueButton != null) continueButton.SetActive(true);
    }

    public void OnContinueClicked()
    {
        uiVisuals.SetActive(false);
        feedbackManager.SetupNextSign("");

        // Stop the video so it doesn't play audio in the background
        if (referenceVideoPlayer != null) referenceVideoPlayer.Stop();
    }
}