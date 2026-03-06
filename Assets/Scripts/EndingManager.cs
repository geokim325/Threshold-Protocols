using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EndingManager
{
    public static void UnlockEnding(int id)
    {
        PlayerPrefs.SetInt("Ending_" + id, 1);
        SaveManager.Instance.SaveGame();
    }

    public static bool IsEndingUnlocked(int id)
    {
        return PlayerPrefs.GetInt("Ending_" + id, 0) == 1;
    }

    public static void ResetAllEndings()
    {
        for (int i = 1; i <= 11; i++)
        {
            PlayerPrefs.DeleteKey("Ending_" + i);
        }
        SaveManager.Instance.SaveGame();
    }
}
