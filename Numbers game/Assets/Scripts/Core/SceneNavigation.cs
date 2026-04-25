using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerNew : MonoBehaviour
{
    public GameObject exitConfirmPanel;
    public string levelSelectSceneName = "LevelSelect";

    public void OnBackButtonPressed()
    {
        if (!GameStart.gameStarted)
        {
            ExitToLevelSelect();
        }
        else
        {
            if (exitConfirmPanel != null)
            {
                exitConfirmPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void ContinuePlaying()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToLevelSelect()
    {
        Time.timeScale = 1f;
        LoadTargetScene(levelSelectSceneName);
    }

    // هذه الدالة التي يجب ربطها بالزر في الـ Inspector
    public void LoadTargetScene(string sceneName)
    {
        Time.timeScale = 1f;

        // حماية: إذا كان الاسم المكتوب هو اسم الزر بالخطأ، سنقوم بتصحيحه
        if (sceneName == "lettersPlanetButton")
        {
            Debug.LogWarning("تنبيه: أنتِ تحاولين تحميل اسم الزر! سأحاول تحميل LevelSelect بدلاً منه.");
            sceneName = "LevelSelect";
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("جاري محاولة فتح المشهد: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}