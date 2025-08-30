using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnBossDefeated();
        }
    }

    public void OnBossDefeated()
    {
        if (StageManager.CurrentStage == 15) //�������� 15�� ���
        {
            Debug.Log("Final Boss Defeated");
            StageManager.CurrentStage = 0; // ����?
            SceneManager.LoadScene("Lobby");
        }
        else // �� �� 5, 10 �������� ����
        {
            Debug.Log("Boss Defeated");
            StageManager.CurrentStage++;
            Debug.Log("���� ��������: " + StageManager.CurrentStage);
            SceneManager.LoadScene("MapSample");
        }
    }

    public void OnBossFailed()
    {
        Debug.Log("Boss Failed");
        Debug.Log("���� ��������: " + StageManager.CurrentStage);
        SceneManager.LoadScene("Lobby");
    }
}
