using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Manager;

public class LobbyManager : MonoBehaviour
{
    void Start()
    {
        // StageManager.CurrentStage = 0; // �ʿ�� �ʱ�ȭ
        Debug.Log("���� ��������: " + StageManager.CurrentStage);
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

            // �� ��ȯ ���� Input ��Ȱ��ȭ
            var inputManager = Object.FindFirstObjectByType<Manager.PlayerInputManager>();
            inputManager?.DisableInput();

            SceneManager.LoadScene("MapSample");
        }
    }
}
