
using UnityEngine;
using UnityEngine.SceneManagement;

public class goToPlanetScene : MonoBehaviour
{
    // عملنا الدالة public عشان تظهر في خيارات الكبسة
    public void GoToPlanetScene()
    {
        SceneManager.LoadScene("planetsScene");
    }
}