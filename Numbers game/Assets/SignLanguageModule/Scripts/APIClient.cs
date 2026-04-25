using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiHandler : MonoBehaviour
{
    public enum ModelMode { Letters, Numbers }

    [Header("API Settings")]
    [Tooltip("Make sure this matches your FastAPI server address")]
    public string serverUrl = "http://127.0.0.1:8001";

    [Tooltip("Toggle this depending on which scene you are in!")]
    public ModelMode currentMode = ModelMode.Letters;

    // Singleton instance for easy access from other scripts
    public static ApiHandler Instance { get; private set; }

    private void Awake()
    {
        // Simple singleton logic. Because we don't use DontDestroyOnLoad,
        // this naturally resets to the correct scene's handler when switching scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    /// Sends the data to the Python FastAPI backend.
    public void RequestPrediction(ModelMode mode,int seqLen, int featSize, List<float> data, Action<PredictResponse> onSuccess, Action<string> onError)
    {
        string endpoint = (mode == ModelMode.Numbers) ? "/predict_numbers" : "/predict_letters";
        StartCoroutine(PostPredictionRoutine(endpoint, seqLen, featSize, data, onSuccess, onError));
    }

    private IEnumerator PostPredictionRoutine(string endpoint, int seqLen, int featSize, List<float> data, Action<PredictResponse> onSuccess, Action<string> onError)
    {
        // Determine the correct endpoint based on the Inspector dropdown
        string fullUrl = serverUrl + endpoint;

        // Build the request object
        PredictRequest reqData = new PredictRequest
        {
            seqLen = seqLen,
            featSize = featSize,
            data = data,
            topk = 3
        };

        // Convert to JSON
        string jsonReq = JsonUtility.ToJson(reqData);

        // Setup the UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonReq);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Debug logs for verification
            Debug.Log($"Sending {data.Count} values");
            Debug.Log($"First 10: {string.Join(",", data.GetRange(0, 10))}");

            // Wait for the response
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Network Error: {request.error}");
            }
            else
            {
                // Parse the response
                string jsonRes = request.downloadHandler.text;
                PredictResponse response = JsonUtility.FromJson<PredictResponse>(jsonRes);

                // Check if Python returned a specific error (like bad_length or labels_mismatch)
                if (!string.IsNullOrEmpty(response.error))
                {
                    string errorMsg = $"Server Error: {response.error}";
                    if (!string.IsNullOrEmpty(response.hint)) errorMsg += $"\nHint: {response.hint}";
                    onError?.Invoke(errorMsg);
                }
                else
                {
                    onSuccess?.Invoke(response);
                }
            }
        }
    }

    // --- Paste this at the bottom of APIClient.cs ---

    [System.Serializable]
    public class PredictRequest
    {
        public int seqLen;
        public int featSize;
        public System.Collections.Generic.List<float> data;
        public int topk;
    }

    [System.Serializable]
    public class PredictResponse
    {
        public string letter;
        public string digit;
        public int index;

        public float confidence;
        public string error;
        public string hint;
    }
}