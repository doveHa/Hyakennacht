using UnityEngine;
using UnityEngine.SceneManagement;

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
}
