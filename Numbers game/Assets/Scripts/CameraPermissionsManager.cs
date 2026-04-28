using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_ANDROID
using UnityEngine.Android; // We need this specifically for Android
#endif

public class CameraPermissionManager : MonoBehaviour
{
    [Tooltip("The exact name of your main game scene")]
    public string nextSceneName = "GameScene";

    void Start()
    {
        StartCoroutine(CheckAndRequestCamera());
    }

    private IEnumerator CheckAndRequestCamera()
    {
        // --- 1. ANDROID CHECK ---
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // Trigger the Android pop-up
            Permission.RequestUserPermission(Permission.Camera);

            // Wait until the user clicks "Allow" or "Deny"
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                yield return null;
            }
        }
        Debug.Log("Permission result: " + Permission.HasUserAuthorizedPermission(Permission.Camera));
#endif

        // --- 2. iOS CHECK ---
#if UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            // Trigger the iOS pop-up
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

#else
        yield return null;
#endif

        // --- 3. PROCEED ---
        // If we made it here, the user said YES! Load the AI game.
        Debug.Log("Camera permission granted! Loading game...");
        SceneManager.LoadScene(nextSceneName);
    }
}