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
        if (Input.GetKeyDown(KeyCode.L))
        {
            StageManager.CurrentStage = 15;
        }
    }

    public void OnBossDefeated()
    {
        if (StageManager.CurrentStage == 15) //스테이지 15인 경우
        {
            gameClearPanel.SetActive(true);
            Debug.Log("Final Boss Defeated");
            return;
        }

        /*            Debug.Log("Boss Defeated");
                    StageManager.CurrentStage++;
                    Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
                    SceneManager.LoadScene("MapSample");
        */

        StageManager.AdvanceStage();
        string nextMap = StageManager.GetMapScene();
        Debug.Log("Boss Defeated -> Loading Map: " + nextMap);
        SceneManager.LoadScene(nextMap);
    }

    public void OnBossFailed()
    {
        Debug.Log("Boss Failed");
        Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
        //SceneManager.LoadScene(1); //WitchLobbyScene
        Debug.Log("Boss Failed -> Returning to Lobby");
        SceneManager.LoadScene(StageManager.GetLobbyScene());
    }

    public void GameClearToLobby()
    {
        gameClearPanel.SetActive(false);
        Debug.Log("Game Clear to Lobby");
        //StageManager.CurrentStage = 0; // 리셋?
        //SceneManager.LoadScene(1); //WitchLobbyScene
        StageManager.CurrentStage = 1;
        SceneManager.LoadScene(StageManager.GetLobbyScene());
    }
}
