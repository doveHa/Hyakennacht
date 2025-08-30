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
        if (StageManager.CurrentStage == 15) //스테이지 15인 경우
        {
            Debug.Log("Final Boss Defeated");
            StageManager.CurrentStage = 0; // 리셋?
            SceneManager.LoadScene("Lobby");
        }
        else // 그 외 5, 10 스테이지 보스
        {
            Debug.Log("Boss Defeated");
            StageManager.CurrentStage++;
            Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
            SceneManager.LoadScene("MapSample");
        }
    }

    public void OnBossFailed()
    {
        Debug.Log("Boss Failed");
        Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
        SceneManager.LoadScene("Lobby");
    }
}
