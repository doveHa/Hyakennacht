using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance { get; private set; }

    [Header("stage clear")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject statsPanel;

    [SerializeField] private RectTransform progressLine;
    [SerializeField] private RectTransform flag;

    [Header("Skill Select")]
    [SerializeField] private GameObject skillSelectPanel;
    [SerializeField] private Button SelectCard1;
    [SerializeField] private Button SelectCard2;
    [SerializeField] private Button SelectCard3;

    private int totalStage = 15;

    private float stageStartTime;
    public int KilledEnemies { get; private set; }
    public int StageCoins { get; private set; }

    private Player player;
    public bool PlayerDied; // 플레이어가 죽었는지 여부 (임시, HP 시스템 알면 제거)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        player = FindFirstObjectByType<Player>(); // CS0618 경고 해결
        PlayerDied = false;
    }

    void Update()
    {
        // 테스트 용도로 Z키로 킬 카운트 증가
        if (Input.GetKeyDown(KeyCode.Z))
        {
            KilledEnemies++;
            StageCoins += 10;
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            PlayerDied = true;
            OnStageEnd(); // 통계창 열기
        }
    }

    public void OnPlayBtnClick()
    {
        Debug.Log("Play button clicked!");
        statsPanel.SetActive(false); // 통계창 닫기

        if (PlayerDied)
        {
            // 플레이어가 죽은 상태라면 스테이지 진행 없이 로비 복귀
            SceneManager.LoadScene(1);
            PlayerDied = false; // 리셋
            return;
        }

        // 카메라에서 TryInteractWithStairs 호출
        PlayerCamera cam = Object.FindFirstObjectByType<PlayerCamera>();
        if (cam != null)
        {
            cam.TryInteractWithStairs();
        }
        else
        {
            Debug.LogWarning("PlayerCamera not found!");
        }
    }

    public void OnCardClick()
    {
        Debug.Log("Card clicked!");
        // Add logic to show card details or perform an action
    }

    private void MoveFlag(int stage)
    {
        if (flag == null || progressLine == null) return;

        float visibleRatio = 0.8f; // 실제 보이는 길이 비율
        float lineWidth = progressLine.rect.width * visibleRatio;
        float ratio = (stage - 1f) / (totalStage - 1f); // 1~15
        float newX = -lineWidth / 2 + ratio * lineWidth;

        Vector3 flagPos = flag.localPosition;
        flag.localPosition = new Vector3(newX, flagPos.y, flagPos.z);
    }

    public void AddKilledEnemy()
    {
        KilledEnemies++;
        if (killText) killText.text = $"{KilledEnemies}";
    }

    public void AddStageCoins(int amount)
    {
        StageCoins += amount;
        if (coinText) coinText.text = $"{StageCoins}";
    }

    public void OnStageStart()
    {
        Debug.Log("Stage started");

        stageStartTime = Time.time;
        KilledEnemies = 0;
        StageCoins = 0;

        if (timeText) timeText.text = "0.0";
        if (killText) killText.text = "0";
        if (coinText) coinText.text = "0";
    }

    public void OnStageEnd()
    {
        float playTime = Time.time - stageStartTime;

        if (timeText) timeText.text = $"{playTime:F1}";
        if (killText) killText.text = $"{KilledEnemies}";
        if (coinText) coinText.text = $"{StageCoins}";

        statsPanel.SetActive(true); // 통계창 열기

        MoveFlag(MapManager.Instance.currentStage);
    }

/*    public void OnSkillSelect(int card1, int card2, int card3)
    {
        skillSelectPanel.SetActive(true);
        SelectCard1.onClick.RemoveAllListeners();
        SelectCard1.onClick.AddListener(() => OnSelectCard(card1));
        SelectCard2.onClick.RemoveAllListeners();
        SelectCard2.onClick.AddListener(() => OnSelectCard(card2));
        SelectCard3.onClick.RemoveAllListeners();
        SelectCard3.onClick.AddListener(() => OnSelectCard(card3));
    }*/
}
