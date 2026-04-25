using UnityEngine;
using UnityEngine.InputSystem;

public class GameStart : MonoBehaviour
{
    public static bool gameStarted = false;

    void Update()
    {
        // استخدام الطريقة الأكثر أماناً للنظام الجديد
        if (!gameStarted && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    public static void StartGame()
    {
        gameStarted = true;
        Debug.Log("Game Started!");
    }
}