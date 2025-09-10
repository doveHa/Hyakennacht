using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Enemy;

public static class StageManager
{
    public static int CurrentStage { get; set; } = 1;
    //public static MapTheme CurrentTheme { get; private set; }
    //public static bool IsLobby { get; set; } = true;

    public static bool IsYokai { get; private set; } = true;

    //게임의 스테이지 관련 설정을 중앙에서 관리
    public const int MaxStage = 6;
    public static readonly List<int> BossStages = new List<int> { 3, 6 };
    public static readonly List<int> SkillSelectStages = new List<int> { 2, 5 };
    public static readonly List<int> ShopStages = new List<int> { 2, 5 };

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
            StairUp();
        }
        else
        {
            StairDown();
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
        //return CurrentStage == 5 || CurrentStage == 10; //|| CurrentStage == 15
        return BossStages.Contains(CurrentStage);
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
        Debug.Log("로비 씬 로드: " + (IsYokai ? "YokaiLobbyScene(Temp)" : "WitchLobbyScene(Temp)"));
        //return "FactionSelectScene";
        return IsYokai ? "YokaiLobbyScene(Temp)" : "WitchLobbyScene(Temp)";
    }

    //추가
    public static bool IsSkillSelectStage()
    {
        return SkillSelectStages.Contains(CurrentStage);
    }

    public static bool IsShopStage()
    {
        return ShopStages.Contains(CurrentStage);
    }

    private static void StairUp()
    {
        EnemySpawner.ToEasy();
        AEnemyStats.ToEasy();
    }
    
    private static void StairDown()
    {
        EnemySpawner.ToHard();
        AEnemyStats.ToHard();
    }
}