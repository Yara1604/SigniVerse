using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SignFeedbackManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The UI Image set to 'Filled' mode")]
    public Image progressFrame;

    [Header("Settings")]
    [Tooltip("How long the user must hold the sign (in seconds)")]
    public float requiredHoldTime = 2f;

    [Tooltip("The sign the user is currently supposed to perform")]
    public string targetSign = "?"; // e.g., the Arabic letter Alif

    [Header("Events")]
    [Tooltip("What happens when the sign is successfully held for the full duration?")]
    public UnityEvent onSignCompleted;

    private float currentHoldTime = 0f;
    private bool isHoldingCorrectSign = false;
    private bool isComplete = false;

    void Update()
    {
        if (isComplete || progressFrame == null) return;

        if (isHoldingCorrectSign)
        {
            // Increase timer smoothly
            currentHoldTime += Time.unscaledDeltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                SignSuccess();
            }
        }
        else
        {
            // Decrease timer if they drop the sign (makes it feel organic rather than instantly resetting)
            currentHoldTime -= Time.unscaledDeltaTime * 2f;
        }

        // Keep the timer between 0 and the max time
        currentHoldTime = Mathf.Clamp(currentHoldTime, 0f, requiredHoldTime);

        // Update the UI Fill
        progressFrame.fillAmount = currentHoldTime / requiredHoldTime;

        // Optional UX Polish: Transition color from white to Green (almost done)
        progressFrame.color = Color.Lerp(Color.white, Color.green, progressFrame.fillAmount);
    }

  
    /// Call this from  API Bridge whenever a new prediction comes in!
    public void UpdatePrediction(string predictedSign)
    {
        if (isComplete) return;

        // Check if the AI's prediction matches the target goal
        isHoldingCorrectSign = (predictedSign == targetSign);
    }

    private void SignSuccess()
    {
        isComplete = true;
        progressFrame.fillAmount = 1f;
        progressFrame.color = Color.green;

        Debug.Log($"<color=cyan>Sign {targetSign} successfully held for {requiredHoldTime} seconds!</color>");

        // Trigger whatever gameplay happens next (e.g., spawn next letter, play sound)
        onSignCompleted?.Invoke();
    }

    // Use this to reset the system for the next letter
    public void SetupNextSign(string newSign)
    {
        targetSign = newSign;
        currentHoldTime = 0f;
        isComplete = false;
        isHoldingCorrectSign = false;
        progressFrame.fillAmount = 0f;
        progressFrame.color = Color.white;
    }
}