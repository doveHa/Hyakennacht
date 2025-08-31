using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


[System.Serializable]
public class SkillData
{
    public int id; //배열 순서 매칭
    public string title;
    public string description;
    public string icon; // JSON에서 Sprite 로드 시, Resources 폴더 사용 추천
    public string assetName; // JSON에 들어가는 에셋 이름
}

[System.Serializable]
public class SkillDataArrayWrapper
{
    public SkillData[] skills;
}

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance { get; private set; }

    [Header("stage clear")] [SerializeField]
    private TMP_Text timeText;

    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject statsPanel;

    [SerializeField] private RectTransform progressLine;
    [SerializeField] private RectTransform flag;

    [Header("Skill Select")] [SerializeField]
    private GameObject skillSelectPanel;

    [SerializeField] private Button SelectCard1;
    [SerializeField] private Button SelectCard2;
    [SerializeField] private Button SelectCard3;

    [SerializeField] private Text cardTitle1;
    [SerializeField] private Text cardDesc1;
    [SerializeField] private Image cardIcon1;

    [SerializeField] private Text cardTitle2;
    [SerializeField] private Text cardDesc2;
    [SerializeField] private Image cardIcon2;

    [SerializeField] private Text cardTitle3;
    [SerializeField] private Text cardDesc3;
    [SerializeField] private Image cardIcon3;

    [Header("Skill Assets")] [SerializeField]
    private SkillBase[] skillAssets; // Inspector에서 모든 스킬 에셋 연결


    private List<SkillData> skillList = new List<SkillData>();

    // 선택한 스킬 저장
    public SkillData SelectedSkill { get; private set; }
    public bool AfterSkillSelect = false;

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

        // 버튼 클릭 이벤트 등록
        SelectCard1.onClick.AddListener(() => OnCardSelected(0));
        SelectCard2.onClick.AddListener(() => OnCardSelected(1));
        SelectCard3.onClick.AddListener(() => OnCardSelected(2));

        LoadSkillsFromJSON();

        player = FindFirstObjectByType<Player>(); // CS0618 경고 해결
        PlayerDied = false;

        skillSelectPanel.SetActive(false);
    }

    void Update()
    {
        // 테스트 용도로 Z키로 킬 카운트 증가
        if (Input.GetKeyDown(KeyCode.Z))
        {
            KilledEnemies++;
            StageCoins += 10;
        }
    }

    public void OnPlayBtnClick()
    {
        Debug.Log("Play button clicked!");
        statsPanel.SetActive(false); // 통계창 닫기

        // 플레이어가 죽은 상태라면 스테이지 진행 없이 로비 복귀
        //SceneManager.LoadScene(1);
        if(PlayerDied)
            Application.Quit();

        /*
        Time.timeScale = 1f;

        SceneManager.LoadScene(StageManager.GetLobbyScene());
*/

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

        // 특정 스테이지에서만 스킬 선택


        if (MapManager.Instance.currentStage == 4 ||
            MapManager.Instance.currentStage == 9 ||
            MapManager.Instance.currentStage == 14)
        {
            ShowSkillSelectPanel();
            AfterSkillSelect = true;
            return; // 스킬 선택 끝날 때까지 stage clear 패널은 안 열림
        }

        statsPanel.SetActive(true); // 통계창 열기

        MoveFlag(MapManager.Instance.currentStage);
    }

    //스킬 선택

    public void ShowSkillSelectPanel()
    {
        ShowRandomSkills(); // 랜덤 스킬 카드 띄우기
        skillSelectPanel.SetActive(true); // 여기서 열림
    }

    private void LoadSkillsFromJSON()
    {
        // JSON은 Resources 폴더에 skill.json으로 가정
        TextAsset json = Resources.Load<TextAsset>("Skills/SkillsInfo");

        if (json != null)
        {
            SkillData[] skills = JsonUtility.FromJson<SkillDataArrayWrapper>(json.text).skills;
            skillList = new List<SkillData>(skills);
        }
        else
        {
            Debug.LogWarning("Skill JSON not found!");
        }
    }

    private void SetCardUI(int cardIndex, SkillData skill)
    {
        Sprite iconSprite = Resources.Load<Sprite>("Skills/SkillIcons/" + skill.icon);
        if (iconSprite == null)
            Debug.LogWarning($"스킬 아이콘 '{skill.icon}'을 Resources/Skills/SkillIcons에서 찾을 수 없습니다.");

        switch (cardIndex)
        {
            case 0:
                cardTitle1.text = skill.title;
                cardDesc1.text = skill.description;
                cardIcon1.sprite = iconSprite;
                break;
            case 1:
                cardTitle2.text = skill.title;
                cardDesc2.text = skill.description;
                cardIcon2.sprite = iconSprite;
                break;
            case 2:
                cardTitle3.text = skill.title;
                cardDesc3.text = skill.description;
                cardIcon3.sprite = iconSprite;
                break;
        }
    }


    private List<SkillData> currentCards = new List<SkillData>();

    private void ShowRandomSkills()
    {
        skillSelectPanel.SetActive(true);

        if (skillList.Count < 3)
        {
            Debug.LogWarning("Not enough skills in JSON!");
            return;
        }

        currentCards.Clear();

        List<int> indices = new List<int>();
        while (indices.Count < 3)
        {
            int rand = Random.Range(0, skillList.Count);
            if (!indices.Contains(rand)) indices.Add(rand);
        }

        currentCards.Add(skillList[indices[0]]);
        currentCards.Add(skillList[indices[1]]);
        currentCards.Add(skillList[indices[2]]);

        SetCardUI(0, currentCards[0]);
        SetCardUI(1, currentCards[1]);
        SetCardUI(2, currentCards[2]);
    }

    private void OnCardSelected(int index)
    {
        SelectedSkill = currentCards[index];
        skillSelectPanel.SetActive(false);

        SkillCaster caster = player.GetComponent<SkillCaster>();

        if (SelectedSkill.id >= 0 && SelectedSkill.id < skillAssets.Length)
        {
            SkillBase skillAsset = skillAssets[SelectedSkill.id];
            if (skillAsset == null)
            {
                Debug.LogWarning(
                    $"skillAssets[{SelectedSkill.id}]가 연결되지 않았습니다. JSON assetName: {SelectedSkill.assetName}");
                return;
            }

            Debug.Log(
                $"선택한 스킬: {SelectedSkill.title}, 배열 인덱스: {SelectedSkill.id}, assetName: {SelectedSkill.assetName}");

            // SkillCaster가 알아서 슬롯 관리
            caster.RegisterNextSkill(skillAsset);
        }
        else
        {
            Debug.LogWarning($"스킬 '{SelectedSkill.title}' ID({SelectedSkill.id})가 배열 범위를 벗어났습니다.");
        }
    }

    /*
     * private void OnCardSelected(int index)
    {
        SelectedSkill = currentCards[index];
        skillSelectPanel.SetActive(false);

        SkillCaster caster = player.GetComponent<SkillCaster>();

        // id 기반으로 배열에서 스킬 찾기
        if (SelectedSkill.id >= 0 && SelectedSkill.id < skillAssets.Length)
        {
            SkillBase skillAsset = skillAssets[SelectedSkill.id];

            if (skillAsset == null)
            {
                Debug.LogWarning($"skillAssets[{SelectedSkill.id}]가 연결되지 않았습니다. JSON assetName: {SelectedSkill.assetName}");
                return;
            }

            Debug.Log($"[Skill Select] 선택한 스킬: {SelectedSkill.title}, 배열 인덱스: {SelectedSkill.id}, assetName: {SelectedSkill.assetName}");
            caster.RegisterSkill(0, skillAsset); // Inspector에서 연결된 SkillBase 그대로 사용
        }
        else
        {
            Debug.LogWarning($"스킬 '{SelectedSkill.title}' ID({SelectedSkill.id})가 배열 범위를 벗어났습니다.");
        }
    }
*/


    // 이름으로 배열 검색
    private SkillBase GetSkillByName(string name)
    {
        foreach (var skill in skillAssets)
        {
            if (skill.name == name) // ScriptableObject 이름과 비교
                return skill;
        }

        return null;
    }
}