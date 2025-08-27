using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("Game Clear")]
    [SerializeField] private GameObject gameClearPanel;

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
            gameClearPanel.SetActive(true);
            Debug.Log("Final Boss Defeated");
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
        SceneManager.LoadScene(1); //WitchLobbyScene
    }

    public void GameClearToLobby()
    {
        gameClearPanel.SetActive(false);
        Debug.Log("Game Clear to Lobby");
        StageManager.CurrentStage = 0; // ����?
        SceneManager.LoadScene(1); //WitchLobbyScene
    }
}
