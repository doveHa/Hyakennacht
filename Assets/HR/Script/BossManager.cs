using System.Collections.Generic;
using System.Threading.Tasks;
using Enemy;
using Enemy.BossStage;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("Game Clear")] [SerializeField]
    private GameObject gameClearPanel;

    public GameObject bossSpawnPoint;

    public GameObject nextRoom;
    
    private List<GameObject> bossObjects;

    private GameObject _bossObject;

    public BossHpBar bossHpBar;
    
    void Awake()
    {
        bossObjects = new List<GameObject>();
    }

    void Start()
    {
        foreach (GameObject boss in BossPrefab.Instance.BossPrefabs)
        {
            bossObjects.Add(boss);
        }
        
        if (StageManager.CurrentStage == 5)
        {
            _bossObject = Instantiate(bossObjects[0], bossSpawnPoint.transform.position, Quaternion.identity);
        }
        else if (StageManager.CurrentStage == 10)
        {
            _bossObject = Instantiate(bossObjects[1], bossSpawnPoint.transform.position, Quaternion.identity);
        }
        _bossObject.GetComponent<EnemyController>().Stage = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
        _bossObject.GetComponent<BossStat>().SetBossHpBar(bossHpBar);
    }

    public void OnBossDefeated()
    {
        if (StageManager.CurrentStage == 15)
        {
            gameClearPanel.SetActive(true);
            return;
        }

        nextRoom.SetActive(true);
        /*
        StageManager.AdvanceStage();
        string nextMap = StageManager.GetMapScene();
        SceneManager.LoadScene(nextMap);*/
    }
    
    public void GameClearToLobby()
    {
        Destroy(GameObject.Find("Player").gameObject);
        Destroy(GameObject.Find("Manager").gameObject);
        gameClearPanel.SetActive(false);
        StageManager.CurrentStage = 1;
        SceneManager.LoadScene(StageManager.GetLobbyScene());
    }
}