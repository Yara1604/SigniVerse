using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;
using UnityEngine;
using static ApiHandler;

public class MediaPipeApiBridge : MonoBehaviour
{
    [Header("MediaPipe Reference")]
    public HandLandmarkerRunner handRunner;

    [Header("API Settings")]
    [Tooltip("How often to ask Python for a prediction (in seconds)")]
    public float requestInterval = 0.5f;

    [Header("Feedback System")]
    public SignFeedbackManager feedbackManager;

    [Header("Model Requirements")]
    public int expectedSeqLen = 90;   // The LSTM expects 90 frames
    public int expectedFeatSize = 42; // The LSTM expects 42 floats (X and Y only)

    private float timer = 0f;
    private bool isWaitingForResponse = false;

    // This queue holds our "video" of the last 90 frames
    private Queue<float[]> sequenceBuffer = new Queue<float[]>();

    void Update()
    {
        if (handRunner == null) return;

        // 1. Capture the hand data EVERY frame to build our 90-frame history
        CaptureHandFrame();

        // 2. Run the timer for the API requests
        timer += Time.unscaledDeltaTime;

        // 3. Only send to Python IF:
        // - Enough time has passed
        // - Python isn't busy
        // - We have successfully recorded exactly 90 frames
        if (timer >= requestInterval && !isWaitingForResponse && sequenceBuffer.Count == expectedSeqLen)
        {
            SendSequenceToAPI();
        }
    }

    private void CaptureHandFrame()
    {
        var result = handRunner.currentResult;

        // If no hands are detected, we could either add a frame of zeros, or just wait.
        // For standard LSTMs, it's usually best to only record when a hand is visible.
        if (result.handLandmarks == null || result.handLandmarks.Count == 0) return;

        var firstHand = result.handLandmarks[0];
        float[] currentFrame = new float[expectedFeatSize];
        int index = 0;

        // Extract ONLY X and Y (Ignore Z) to get exactly 42 features
        foreach (var landmark in firstHand.landmarks)
        {
            if (index >= expectedFeatSize) break; // Safety check

            currentFrame[index++] = landmark.x;
            currentFrame[index++] = landmark.y;
        }

        // Add this new frame to our history
        sequenceBuffer.Enqueue(currentFrame);

        // If we have more than 90 frames, throw away the oldest frame (Sliding Window)
        if (sequenceBuffer.Count > expectedSeqLen)
        {
            sequenceBuffer.Dequeue();
        }
    }

    private void SendSequenceToAPI()
    {
        List<float> flattenedData = new List<float>();

        // Flatten all 90 frames into one giant list
        foreach (var frame in sequenceBuffer)
        {
            flattenedData.AddRange(frame);
        }

        //for (int i = 1; i < flattenedData.Count; i += 2)
        //{
        //    flattenedData[i] = 1f - flattenedData[i];
        //}


        isWaitingForResponse = true;
        timer = 0f;

        // Send to FastAPI
        ApiHandler.Instance.RequestPrediction(
            mode: ApiHandler.Instance.currentMode,
            seqLen: expectedSeqLen,
            featSize: expectedFeatSize,
            data: flattenedData,
            onSuccess: HandleSuccess,
            onError: HandleError
        );
    }

    private void HandleSuccess(PredictResponse response)
    {
        isWaitingForResponse = false;

        // Note: I updated this to look for 'letter' or 'digit' based on your Python code!
        string resultText = string.IsNullOrEmpty(response.letter) ? response.digit : response.letter;

        if (feedbackManager != null)
        {
            feedbackManager.UpdatePrediction(resultText);
        }

        Debug.Log($"<color=green>API Success!</color> Prediction: {resultText} | Confidence: {response.confidence}");
    }

    private void HandleError(string errorMessage)
    {
        isWaitingForResponse = false;
        Debug.LogError($"<color=red>API Failed:</color> {errorMessage}");
    }
}