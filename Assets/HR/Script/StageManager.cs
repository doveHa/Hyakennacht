using UnityEngine;

public static class StageManager
{
    public static int CurrentStage { get; set; } = 1;

    public static void AdvanceStage(bool isStairUp)
    {
        CurrentStage++;
        Debug.Log("���������� �����߽��ϴ�. ���� ��������: " + CurrentStage);

        AdjustDifficulty(isStairUp);
    }

    private static void AdjustDifficulty(bool isStairUp)
    {
        if (isStairUp)
        {
            Debug.Log("��� Up: ���̵� ���");
        }
        else
        {
            Debug.Log("��� Down: ���̵� �϶�");
        }
    }
}
