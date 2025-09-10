using System;
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

    public GameObject bossSpawnPoint, playerSpawnPoint;

    public GameObject nextRoom;

    private GameObject _middleBossPrefab, _finalBossPrefab, _bossObject;

    public BossHpBar bossHpBar;

    void Awake()
    {
    }

    void Start()
    {
        if (StageManager.IsYokai)
        {
            _middleBossPrefab = AddressableManager.Manager.GetPrefabByName(Constant.EnemyName.YOKAI_MIDDLE_BOSS);
            _finalBossPrefab = AddressableManager.Manager.GetPrefabByName(Constant.EnemyName.YOKAI_FINAL_BOSS);
        }
        else
        {
            _middleBossPrefab = AddressableManager.Manager.GetPrefabByName(Constant.EnemyName.WITCH_MIDDLE_BOSS);
            _finalBossPrefab = AddressableManager.Manager.GetPrefabByName(Constant.EnemyName.WITCH_FINAL_BOSS);
        }

        if (StageManager.CurrentStage == Constant.Stage.MIDDLE_BOSS)
        {
            _bossObject = Instantiate(_middleBossPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
        }
        else if (StageManager.CurrentStage == Constant.Stage.FINAL_BOSS)
        {
            _bossObject = Instantiate(_finalBossPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
        }

        GameManager.Manager.Player.transform.position = playerSpawnPoint.transform.position;
        _bossObject.GetComponent<EnemyController>().Stage = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
        _bossObject.GetComponent<BossStat>().SetBossHpBar(bossHpBar);
    }

    public void OnBossDefeated()
    {
        if (StageManager.CurrentStage == Constant.Stage.FINAL_BOSS)
        {
            gameClearPanel.SetActive(true);
            return;
        }

        nextRoom.SetActive(true);
        foreach (UpDownStair script in nextRoom.GetComponentsInChildren<UpDownStair>())
        {
            script.IsEndStage = true;
        }
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