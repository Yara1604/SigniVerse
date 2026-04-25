using UnityEngine;

public static class LevelUnlockManager
{
    public static bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1)
            return true; // أول مرحلة دائمًا مفتوحة

        return PlayerPrefs.GetInt("LevelUnlocked_" + levelNumber, 0) == 1;
    }

    public static void UnlockLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("LevelUnlocked_" + levelNumber, 1);
        PlayerPrefs.Save();
    }
}