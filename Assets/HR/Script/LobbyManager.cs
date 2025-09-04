using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Manager;

public class LobbyManager : MonoBehaviour
{
    public bool isYokai = true;

    void Start()
    {
        // StageManager.CurrentStage = 0; // �ʿ�� �ʱ�ȭ
        StageManager.SetTheme(isYokai);
        Debug.Log("���� ��������: " + StageManager.CurrentStage);
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

        // �� ��ȯ ���� Input ��Ȱ��ȭ
        var inputManager = Object.FindFirstObjectByType<Manager.PlayerInputManager>();
        inputManager?.DisableInput();

        //string nextMap = StageManager.GetMapScene();
        string nextMap = "WitchMap(Temp)";
        Debug.Log("Lobby -> Loading Map: " + nextMap);
        SceneManager.LoadScene(nextMap);
    }
}
