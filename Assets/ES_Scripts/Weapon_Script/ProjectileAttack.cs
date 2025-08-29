using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour, IWeaponBehavior
{
    private WeaponData data;
    private Transform firePoint;
    private GameObject soundWave;

    public void Initialize(WeaponData data, Transform firePoint)
    {
        this.data = data;
        this.firePoint = firePoint;

        if (data.weaponName == "무령")
        {
            soundWave = Resources.Load<GameObject>("Effects/소리 파동");
            if (soundWave == null)
                Debug.LogWarning("SoundWave 프리팹을 찾을 수 없습니다.");
        }
    }

    public void Attack()
    {
        Debug.Log("어택 호출 완료");

        if (data.prefab == null)
        {
            Debug.LogWarning("무기 프리팹이 없습니다.");
            return;
        }

        Vector2 direction = new Vector2(firePoint.parent.localScale.x, 0).normalized;

        Vector3 Pos = firePoint.position;
        if (data.weaponName == "긴 지팡이")
        {
            Pos += Vector3.up * 1f;
        }

        if (data.weaponName == "무령" && soundWave != null)
        {
            Vector3 spawnPos = new Vector3(firePoint.position.x, firePoint.position.y + 0.5f, firePoint.position.z);
            GameObject wave = GameObject.Instantiate(soundWave, spawnPos, Quaternion.identity);
            Destroy(wave, 1f);
            //CameraShake();
        }

        GameObject proj = GameObject.Instantiate(data.prefab, Pos, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        // 스프라이트 렌더러를 찾아 좌우 반전
        SpriteRenderer projSprite = proj.GetComponentInChildren<SpriteRenderer>();
        if (projSprite != null)
        {
            // 플레이어의 x 스케일이 음수면 스프라이트를 뒤집음
            projSprite.flipX = firePoint.parent.localScale.x < 0;
        }

        LongStaff staff = proj.GetComponent<LongStaff>();
        if (staff != null)
        {
            staff.SetDamage(data.baseDamage);
            staff.Launch(direction, data.attackSpeed);
            return;
        }

        if (data.weaponName == "포션")
        {
            rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 1.5f;

                Vector2 throwVelocity = new Vector2(direction.x * 5f, 5f); 
                rb.velocity = throwVelocity;
            }

            Potion potion = proj.GetComponent<Potion>();
            if (potion != null)
            {
                potion.potionType = PotionEffectHelper.GetRandomEffect();
            }

            return;
        }

        // 애니메이션 처리
        Animator weaponAnimator = proj.GetComponent<Animator>();
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("isAttack", true);
        }

        // 직선 방향으로 발사
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * data.attackSpeed;
        }

        Soul soul = proj.GetComponent<Soul>();
        if (soul != null)
        {
            soul.SetDamage(data.baseDamage);
        }
        else
        {
            Bell bell = proj.GetComponent<Bell>();
            if (bell != null)
            {
                float xDir = Mathf.Sign(firePoint.parent.localScale.x);
                bell.SetDamage(data.baseDamage);
                bell.SetDirection(xDir);
            }
            else
            {
                Projectile p = proj.GetComponent<Projectile>();
                if (p != null)
                {
                    p.SetDamage(data.baseDamage);
                    p.SetWeaponName(data.weaponName);
                }
                    
            }
        }
    }

    private void CameraShake()
    {
        Debug.Log("흔들림 효과 발생");
    }

}
