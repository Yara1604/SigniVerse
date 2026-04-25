using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    // الدالة اللي رح تفتح اللعبة الثانية
    public void OpenAlphabetGame()
    {
        // تأكد إنك كاتب اسم المشهد بالظبط (مثلاً Level2 أو SampleScene)
        SceneManager.LoadScene("SampleScene");
    }

    // دالة اختيارية للرجوع للمتجر أو القائمة الرئيسية
    public void ReturnToMain()
    {
        SceneManager.LoadScene("mainMenu");
    }
}