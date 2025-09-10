using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // �÷��̾� Transform
    public Vector3 offset = new Vector3(0, 5, -10);
    //public MapManager mapManager; // Inspector���� �Ҵ�

    void Awake()
    {
        // �� ��ȯ�� ������ �ڵ� ȣ��ǵ��� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // �޸� ���� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.LookAt(player);
        }
    }

    void Update()
    {
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // Tag ��� ����
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private Vector3 GetPlayerBottomPosition()
    {
        // �÷��̾� Collider �ϴ� ���� ��ġ
        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null)
            return col.bounds.min + Vector3.up * 0.05f; // �ణ ���� offset
        else
            return player.position;
    }

}