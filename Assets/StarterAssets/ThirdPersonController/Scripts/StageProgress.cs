using UnityEngine;

public static class StageProgress
{
    public static int GetUnlockedStageIndex()
    {
        return PlayerPrefs.GetInt("UnlockedStage", 0); 
    }

    public static void UnlockStage(int index)
    {
        int current = GetUnlockedStageIndex();
        if (index > current)
        {
            PlayerPrefs.SetInt("UnlockedStage", index);
            PlayerPrefs.Save();
        }
    }
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedStage");
    }
    public static void MarkStageCleared(int stageIndex)
    {
        PlayerPrefs.SetInt($"Stage_{stageIndex}_Cleared", 1);
        PlayerPrefs.Save();
    }
    public static bool IsStageCleared(int stageIndex)
    {
        return PlayerPrefs.GetInt($"Stage_{stageIndex}_Cleared", 0) == 1;
    }
}
