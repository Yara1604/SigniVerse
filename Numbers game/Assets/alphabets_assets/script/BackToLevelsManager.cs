using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelsManager : MonoBehaviour
{
    [Header("واجهة المستخدم")]
    public GameObject exitConfirmPanel;

    [Header("إعدادات المشهد")]
    public string levelSelectSceneName = "LevelSelect";

    // --- الدالة الجديدة للخروج من اللعبة تماماً ---
    public void QuitGameCompletely()
    {
        Debug.Log("جاري إغلاق اللعبة...");
        Application.Quit(); // تغلق اللعبة (تعمل فقط في النسخة المبنية Build وليس المحرر)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // لإيقاف اللعبة داخل المحرر (للـ Testing)
#endif
    }

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
            else
            {
                ExitToLevelSelect();
            }
        }
    }

    public void ContinuePlaying()
    {
        if (exitConfirmPanel != null)
            exitConfirmPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToLevelSelect()
    {
        Time.timeScale = 1f;
        GameStart.gameStarted = false;
        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(levelSelectSceneName)) return;

        string cleanName = levelSelectSceneName.Trim();
        if (Application.CanStreamedLevelBeLoaded(cleanName))
        {
            SceneManager.LoadScene(cleanName);
        }
    }
}