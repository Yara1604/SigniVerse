using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public WinUIManager uiManager;
    public int currentLevelNumber = 1;

    private bool finished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished) return;

        if (other.CompareTag("Player"))
        {
            finished = true;

            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.FinishLevel();
            }

            if (LevelStarsManager.Instance != null)
            {
                LevelStarsManager.Instance.SaveCollectedStars();
            }

            LevelUnlockManager.UnlockLevel(currentLevelNumber + 1);

            GameStart.gameStarted = false;

            if (uiManager != null)
            {
                uiManager.ShowWinPanel();
            }
        }
    }
}