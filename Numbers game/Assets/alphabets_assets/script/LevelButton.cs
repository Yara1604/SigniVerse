using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    public string sceneName;

    public Button button;
    public Image levelImage;
    public GameObject lockIcon;

    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color unlockedColor = Color.white;

    void Start()
    {
        bool unlocked = LevelUnlockManager.IsLevelUnlocked(levelNumber);

        if (button != null)
            button.interactable = unlocked;

        if (levelImage != null)
            levelImage.color = unlocked ? unlockedColor : lockedColor;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);
    }

    public void OpenLevel()
    {
        if (LevelUnlockManager.IsLevelUnlocked(levelNumber))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}