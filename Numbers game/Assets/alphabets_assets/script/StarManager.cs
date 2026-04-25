using UnityEngine;
using TMPro;

public class LevelStarsManager : MonoBehaviour
{
    public static LevelStarsManager Instance;

    public int currentLevelStars = 0;
    public TextMeshProUGUI starsText;

    private bool starsSaved = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateStarsUI();
    }

    public void AddStar()
    {
        currentLevelStars++;
        UpdateStarsUI();
    }

    public void UpdateStarsUI()
    {
        if (starsText != null)
        {
            starsText.text = currentLevelStars.ToString();
        }
    }

    public void SaveCollectedStars()
    {
        if (starsSaved) return;

        //StarsSaveManager.AddStars(currentLevelStars);
        GameData.AddStars(currentLevelStars);
        starsSaved = true;
    }
}