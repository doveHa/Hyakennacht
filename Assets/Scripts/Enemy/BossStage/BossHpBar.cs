using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public Image currentHealthBar;
    private float _hitPoint;
    private float _maxHitPoint;
    
    public void SetHp(float hp)
    {
        _maxHitPoint = hp;
        _hitPoint = _maxHitPoint;
        UpdateHealthBar();
    }
    
    public void TakeDamage(float Damage)
    {
        _hitPoint -= Damage;
        if (_hitPoint < 1)
            _hitPoint = 0;

        UpdateHealthBar();

    }
	
    private void UpdateHealthBar()
    {
        float ratio = _hitPoint / _maxHitPoint;
        currentHealthBar.rectTransform.localPosition = new Vector3(currentHealthBar.rectTransform.rect.width * ratio - currentHealthBar.rectTransform.rect.width, 0, 0);
    }
}