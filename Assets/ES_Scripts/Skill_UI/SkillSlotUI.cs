using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;                  // 아이콘
    public Image cooldownFill;          // Image.type = Filled, FillMethod = Radial360
    public TextMeshProUGUI cdText;      // "1.2" 같은 숫자
    public TextMeshProUGUI keyText;     // "F","G","H" 등

    Color _iconDefault;
    [SerializeField] Color disabledTint = new(1, 1, 1, 0.35f);

    void Awake()
    {
        if (icon) _iconDefault = icon.color;
        if (cooldownFill) cooldownFill.fillAmount = 0f;
        if (cdText) cdText.text = "";
    }

    public void SetIcon(Sprite s)
    {
        if (!icon) return;
        icon.sprite = s;
        icon.enabled = (s != null);
    }

    public void SetHotkey(string keyLabel)
    {
        if (keyText) keyText.text = keyLabel ?? "";
    }

    /// <summary>
    /// remain>0이면 쿨타임 UI 갱신, 0이면 숨김
    /// </summary>
    public void UpdateCooldown(float remain, float total)
    {
        if (!cooldownFill || !cdText) return;

        if (remain > 0f && total > 0.01f)
        {
            cooldownFill.fillAmount = Mathf.Clamp01(remain / total);
            cdText.text = (remain >= 1f) ? Mathf.CeilToInt(remain).ToString()
                                         : remain.ToString("0.0");
            cdText.enabled = true;
        }
        else
        {
            cooldownFill.fillAmount = 0f;
            cdText.text = "";
            cdText.enabled = false;
        }
    }

    public void SetUsableVisual(bool usable)
    {
        if (!icon) return;
        icon.color = usable ? _iconDefault : disabledTint;
    }
}
