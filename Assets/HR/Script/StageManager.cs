using UnityEngine;
using UnityEngine.SceneManagement;

public static class StageManager
{
    public static int CurrentStage { get; set; } = 1;
    //public static MapTheme CurrentTheme { get; private set; }
    //public static bool IsLobby { get; set; } = true;

    public static bool IsYokai { get; private set; } = true;


    public static void AdvanceStage(bool isStairUp)
    {
        CurrentStage++;
        Debug.Log("스테이지가 증가했습니다. 현재 스테이지: " + CurrentStage);

        AdjustDifficulty(isStairUp);
    }

    private static void AdjustDifficulty(bool isStairUp)
    {
        if (isStairUp)
        {
            Debug.Log("계단 Up: 난이도 상승");
        }
        else
        {
            Debug.Log("계단 Down: 난이도 하락");
        }
    }

    //마녀/요괴 씬

    public static void SetTheme(bool yokai)
    {
        IsYokai = yokai;
    }

    public static void AdvanceStage()
    {
        CurrentStage++;
    }

    public static bool IsBossStage()
    {
        return CurrentStage == 5 || CurrentStage == 10 || CurrentStage == 15;
    }

    public static string GetMapScene()
    {
        return IsYokai ? "YokaiMap" : "WitchMap";
    }

    public static string GetBossScene()
    {
        return IsYokai ? "YokaiBoss" : "WitchBoss";
    }

    public static string GetLobbyScene()
    {
        return "FactionSelectScene";
        //return IsYokai ? "YokaiLobbyScene(Temp)" : "WitchLobbyScene(Temp)";
    }
}
/*
public enum MapTheme
{
    Yokai,
    Witch
}*/