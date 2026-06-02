using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;
using UnityEngine;
using static APIClient;

public class MediaPipeAPIBridge : MonoBehaviour
{
    [Header("MediaPipe Reference")]
    public HandLandmarkerRunner handRunner;

    [Header("API Settings")]
    [Tooltip("How often to ask Python for a prediction (in seconds)")]
    public float requestInterval = 0.3f;

    [Header("Feedback System")]
    public SignFeedbackManager feedbackManager;

    [Header("Model Requirements")]
    public int expectedSeqLen = 90;   // The LSTM expects 90 frames
    public int expectedFeatSize = 42; // The LSTM expects 42 floats (X and Y only)

    [Header("Time Scale Fix")]
    [Tooltip("Match this to your Python TARGET_FPS!")]
    public float targetCaptureFPS = 30f;
    private float captureTimer = 0f;

    public bool useTwoHands = false;

    private float timer = 0f;
    private bool isWaitingForResponse = false;

    // This queue holds our "video" of the last 90 frames
    private Queue<float[]> sequenceBuffer = new Queue<float[]>();

    void Update()
    {
        if (handRunner == null) return;


        // Only capture a frame at exactly 30 FPS, ignoring Unity's actual framerate
        captureTimer += Time.unscaledDeltaTime;
        float timeBetweenFrames = 1f / targetCaptureFPS;

        if (captureTimer >= timeBetweenFrames)
        {
            CaptureHandFrame();
            captureTimer -= timeBetweenFrames; // Reset timer accurately
        }

        // 2. Run the timer for the API requests
        timer += Time.unscaledDeltaTime;

        // 3. Only send to Python IF ready
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

        try {
            // Extract first hand
            if (!useTwoHands)
            {
                var firstHand = result.handLandmarks[0];
                float[] currentFrame = new float[expectedFeatSize];
                int index = 0;

                // Extract ONLY X and Y (Ignore Z) to get exactly 42 features
                for (int j = 0; j < firstHand.landmarks.Count; j++)
                {
                    if (index >= expectedFeatSize) break; // Safety check
                    var landmark = firstHand.landmarks[j];
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

            else // for two hands on laptop
            {
                float[] currentFrame = new float[126];
                int halfSize = 126 / 2;

                // Keep track of which parking spaces are full
                bool slot0Filled = false;
                bool slot1Filled = false;

                // Use handLandmarks or handWorldLandmarks depending on what you decided!
                var landmarksList = result.handWorldLandmarks;

                if (landmarksList != null && result.handedness != null)
                {
                    // Loop through the detected hands (up to 2)
                    for (int i = 0; i < Mathf.Min(landmarksList.Count, 2); i++)
                    {
                        var hand = landmarksList[i];

                        // Safely get the label. Default to "right" if MediaPipe glitches and returns null.
                        string label = "right";
                        if (i < result.handedness.Count && result.handedness[i].categories != null && result.handedness[i].categories.Count > 0)
                        {
                            label = result.handedness[i].categories[0].categoryName.ToLower();
                        }

                        // Flipping FOR MOBILE
                        if (label == "right")
                            label = "left";
                        else if (label == "left")
                            label = "right";


                        int offset = 0;

                        // SMART PARKING LOGIC
                        if (label == "right" && !slot0Filled)
                        {
                            offset = 0;
                            slot0Filled = true;
                        }
                        else if (label == "left" && !slot1Filled)
                        {
                            offset = halfSize;
                            slot1Filled = true;
                        }
                        else if (!slot0Filled) // If its preferred slot is taken, force it into Slot 0
                        {
                            offset = 0;
                            slot0Filled = true;
                        }
                        else // Force it into Slot 1
                        {
                            offset = halfSize;
                            slot1Filled = true;
                        }

                        int index = offset;

                        // Extract features
                        for (int j = 0; j < hand.landmarks.Count; j++)
                        {
                            if (index >= offset + halfSize) break; // Safety check
                            var landmark = hand.landmarks[j];
                            currentFrame[index++] = landmark.x;
                            currentFrame[index++] = landmark.y;
                            currentFrame[index++] = landmark.z;

                        }
                    }
                }

                sequenceBuffer.Enqueue(currentFrame);

                if (sequenceBuffer.Count > expectedSeqLen)
                {
                    sequenceBuffer.Dequeue();
                }
            }


        }
        catch (System.Exception)
        {
            // A multithreading collision occurred! 
            // We just return safely and let the next frame handle it.
            return;
        }
    }

    private void SendSequenceToAPI()
    {
        List<float> flattenedData = new List<float>();

        // Flatten all frames into one giant list
        foreach (var frame in sequenceBuffer)
        {
            flattenedData.AddRange(frame);
        }

        //// FOR MOBILE, TO CHANGE FLIP Y AXIS (Different coordinate system)
        if (!useTwoHands)
        {
            for (int i = 1; i < flattenedData.Count; i += 2)
            {
                flattenedData[i] = 1f - flattenedData[i];
            }
        }
        else
        {
            for (int i = 1; i < flattenedData.Count; i += 3)
            {
                flattenedData[i] = -flattenedData[i];
            }
        }



        isWaitingForResponse = true;
        timer = 0f;

        // Send to FastAPI
        APIClient.Instance.RequestPrediction(
            mode: APIClient.Instance.currentMode,
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
        string resultText = "";

        if (!string.IsNullOrEmpty(response.home))
        {
            resultText = response.home;
        }
        else if (!string.IsNullOrEmpty(response.letter))
        {
            resultText = response.letter;
        }
        else
        {
            resultText = response.digit;
        }

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