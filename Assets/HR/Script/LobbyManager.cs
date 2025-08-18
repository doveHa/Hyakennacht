using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    void Start()
    {
        // StageManager.CurrentStage = 0; // 필요시 초기화
        Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Lobby To Map");
            SceneManager.LoadScene("MapSample");
        }
    }
}
