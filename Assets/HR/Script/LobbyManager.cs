using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Manager;

public class LobbyManager : MonoBehaviour
{
    public bool isYokai = true;

    void Start()
    {
        // StageManager.CurrentStage = 0; // 필요시 초기화
        StageManager.SetTheme(isYokai);
        Debug.Log("현재 스테이지: " + StageManager.CurrentStage);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Lobby To Map");
            string nextMap = StageManager.GetMapScene();
            Debug.Log("Lobby -> Loading Map: " + nextMap);
            SceneManager.LoadScene(nextMap);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 씬 전환 전에 Input 비활성화
        var inputManager = Object.FindFirstObjectByType<Manager.PlayerInputManager>();
        inputManager?.DisableInput();

        string nextMap = StageManager.GetMapScene();
        Debug.Log("Lobby -> Loading Map: " + nextMap);
        SceneManager.LoadScene(nextMap);
    }
}
