using UnityEngine;

public static class StageManager
{
    public static int CurrentStage { get; set; } = 1;

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
}
