using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Manager;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the game");

            // 씬 전환 전에 Input 비활성화
            var inputManager = Object.FindFirstObjectByType<Manager.PlayerInputManager>();
            inputManager?.DisableInput();

            SceneManager.LoadScene("MapSample");
        }
    }
}
