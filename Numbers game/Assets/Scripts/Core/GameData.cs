using UnityEngine;

public static class GameData
{
    private const string TotalStarsKey = "TotalGlobalStars";
    private const string SelectedAvatarKey = "SelectedAvatarIndex";

    public static int GetTotalStars()
    {
        return PlayerPrefs.GetInt(TotalStarsKey, 0);
    }

    public static void SetTotalStars(int value)
    {
        PlayerPrefs.SetInt(TotalStarsKey, value);
        PlayerPrefs.Save();
    }

    public static void AddStars(int amount)
    {
        int current = GetTotalStars();
        current += amount;
        SetTotalStars(current);
    }

    public static bool IsItemPurchased(string itemName, int index)
    {
        string key = GetItemKey(itemName, index);
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public static void SetItemPurchased(string itemName, int index, bool purchased)
    {
        string key = GetItemKey(itemName, index);
        PlayerPrefs.SetInt(key, purchased ? 1 : 0);
        PlayerPrefs.Save();
    }

    private static string GetItemKey(string itemName, int index)
    {
        return $"Item_{itemName}_{index}";
    }

    public static int GetSelectedAvatarIndex()
    {
        return PlayerPrefs.GetInt(SelectedAvatarKey, 0);
    }

    public static void SetSelectedAvatarIndex(int index)
    {
        PlayerPrefs.SetInt(SelectedAvatarKey, index);
        PlayerPrefs.Save();
    }

    public static void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}