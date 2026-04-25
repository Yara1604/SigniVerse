using UnityEngine;
using UnityEngine.Video; // <-- Added to support videos!

public class BridgeTrigger : MonoBehaviour
{
    [Header("Bridge Connections")]
    public BridgeClickable bridgeClickable;

    [Header("AI Module Connections")]
    [Tooltip("Drag your imported SignFeedbackPanel Canvas here")]
    public SignGameManager signGameManager;

    [Header("Sign Settings")]
    [Tooltip("What the AI looks for (e.g., ا)")]
    public string aiTargetLetter = "ا";

    [Tooltip("What the UI displays (e.g., حرف الألف)")]
    public string displayText = "ا";

    [Tooltip("The video demonstrating the sign")]
    public VideoClip signVideo;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // 1. إيقاف السيارة (Stop the car)
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.StopAtBridge();
            }

            // 2. إخبار سكريبت الجسر أن اللاعب أصبح قريباً
            if (bridgeClickable != null)
            {
                bridgeClickable.SetPlayerNearby(true);
            }

            // 3. فتح لوحة الذكاء الاصطناعي (Open the AI Panel!)
            if (signGameManager != null)
            {
                signGameManager.OpenPuzzle(aiTargetLetter, displayText, signVideo);

                // Pause the background game just like we did in the Stars game
                // Time.timeScale = 0f;

                Debug.Log($"Car Stopped! AI Panel opened for {aiTargetLetter}");
            }
            else
            {
                Debug.LogError("Sign Game Manager is not assigned on the bridge!");
            }
        }
    }

    // --- We will call this from the Continue Button later! ---
    public void OnBridgeSignCompleted()
    {
        Time.timeScale = 1f; // Unpause the game

        // Put whatever code you use to lower the bridge and start the car here!
        // For example: bridgeClickable.LowerBridge();
    }
}