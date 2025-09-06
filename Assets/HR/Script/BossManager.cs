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

    private List<GameObject> bossObjects;

    private GameObject _bossObject;

    public BossHpBar bossHpBar;
    
    void Awake()
    {
        bossObjects = new List<GameObject>();
    }

    async void Start()
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
        await _bossObject.GetComponent<AEnemyStats>().SetStat();
        _bossObject.GetComponent<BossStat>().SetBossHpBar(bossHpBar);
    }

    void Update()
    {
    
    }

    public void OnBossDefeated()
    {
        if (StageManager.CurrentStage == 15) //�������� 15�� ���
        {
            gameClearPanel.SetActive(true);
            Debug.Log("Final Boss Defeated");
            return;
        }

        /*            Debug.Log("Boss Defeated");
                    StageManager.CurrentStage++;
                    Debug.Log("���� ��������: " + StageManager.CurrentStage);
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
        Debug.Log("���� ��������: " + StageManager.CurrentStage);
        //SceneManager.LoadScene(1); //WitchLobbyScene
        Debug.Log("Boss Failed -> Returning to Lobby");
        SceneManager.LoadScene(StageManager.GetLobbyScene());
    }

    public void GameClearToLobby()
    {
        gameClearPanel.SetActive(false);
        Debug.Log("Game Clear to Lobby");
        //StageManager.CurrentStage = 0; // ����?
        //SceneManager.LoadScene(1); //WitchLobbyScene
        StageManager.CurrentStage = 1;
        SceneManager.LoadScene(StageManager.GetLobbyScene());
    }

    void OnEnable()
    {
    }
}