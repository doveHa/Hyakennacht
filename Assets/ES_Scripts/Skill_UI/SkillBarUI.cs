using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillBarUI : MonoBehaviour
{
    [Header("Refs")]
    public SkillCaster caster;
    public SkillSlotUI slotPrefab;
    public Transform container;          // HorizontalLayoutGroup 권장

    [Header("Keys (선택)")]
    public string[] keyLabels = new[] { "F", "G", "H", "J", "K" };

    [Header("Empty Icon (선택)")]
    public Sprite emptyIcon;

    List<SkillSlotUI> _slots = new();

    void Start()
    {
        if (!caster) caster = FindFirstObjectByType<SkillCaster>();
        Rebuild();
    }

    public void Rebuild()
    {
        foreach (Transform c in container) Destroy(c.gameObject);
        _slots.Clear();

        var skills = caster ? caster.GetSlots() : null;
        if (skills == null || slotPrefab == null || container == null) return;

        for (int i = 0; i < skills.Length; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            _slots.Add(ui);

            var s = skills[i];
            ui.SetIcon(s ? s.icon : emptyIcon);
            ui.SetHotkey(i < keyLabels.Length ? keyLabels[i] : "");
        }
    }

    void Update()
    {
        if (!caster || _slots.Count == 0) return;

        var skills = caster.GetSlots();
        float gcdRemain = caster.GetGCD();
        float gcdTotal = caster.GetGCDTotal();

        for (int i = 0; i < _slots.Count; i++)
        {
            var ui = _slots[i];
            var s = (skills != null && i < skills.Length) ? skills[i] : null;

            float remain = 0f, total = 0f;

            if (s != null)
            {
                // 1) 본인 스킬 쿨다운이 먼저 표시
                remain = caster.GetCooldown(s.skillId);
                total = s.cooldown;

                // 2) 본인 쿨이 없고, GCD가 돌고 있고, 이 스킬이 GCD를 사용한다면 GCD 표시
                if (remain <= 0f && s.useGCD && gcdRemain > 0f)
                {
                    remain = gcdRemain;
                    total = gcdTotal;
                }

                // 버튼 비활성화/회색 처리등 원하면 여기서
                ui.SetUsableVisual(remain <= 0f);
            }
            else
            {
                // 빈 슬롯: GCD만 보여주고 싶다면 여기에
                if (gcdRemain > 0f) { remain = gcdRemain; total = gcdTotal; }
                ui.SetUsableVisual(remain <= 0f);
            }

            ui.UpdateCooldown(remain, total);
        }
    }
}
