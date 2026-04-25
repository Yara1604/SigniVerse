using UnityEngine;

public static class StarsSaveManager
{
    private const string TotalStarsKey = "TotalStars";

    public static int GetTotalStars()
    {
        return PlayerPrefs.GetInt(TotalStarsKey, 0);
    }

    public static void AddStars(int amount)
    {
        int currentTotal = GetTotalStars();
        currentTotal += amount;

        PlayerPrefs.SetInt(TotalStarsKey, currentTotal);
        PlayerPrefs.Save();
    }

    public static void ResetAllStars()
    {
        PlayerPrefs.SetInt(TotalStarsKey, 0);
        PlayerPrefs.Save();
    }
}