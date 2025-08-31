using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour, IWeaponBehavior, IFlippableWeapon
{
    private WeaponData data;
    private Transform firePoint;
    private GameObject soundWave;
    private bool isLeft;

    [SerializeField] bool invertInput = true;

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

    public void SetFacingDirection(bool isLeft)
    {
        this.isLeft = invertInput ? !isLeft : isLeft;
    }

    public void Attack()
    {
        Debug.Log("어택 호출 완료");

        if (data.prefab == null)
        {
            Debug.LogWarning("무기 프리팹이 없습니다.");
            return;
        }

        float directionSign = isLeft ? -1f : 1f;

        Vector3 spawnOffset = new Vector3(directionSign * 0.5f, 0, 0);
        Vector3 Pos = firePoint.position + spawnOffset;

        if (data.weaponName == "긴 지팡이")
        {
            Pos += Vector3.up * 1f;
        }

        if (data.weaponName == "무령" && soundWave != null)
        {
            Vector3 soundWaveSpawnPos = new Vector3(firePoint.position.x, firePoint.position.y + 0.5f, firePoint.position.z);
            GameObject wave = GameObject.Instantiate(soundWave, soundWaveSpawnPos, Quaternion.identity);
            Destroy(wave, 1f);
        }

        GameObject proj = GameObject.Instantiate(data.prefab, Pos, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        SpriteRenderer projSprite = proj.GetComponentInChildren<SpriteRenderer>();
        if (projSprite != null)
        {
            projSprite.flipX = isLeft;
        }

        Vector2 direction = new Vector2(directionSign, 0f);

        LongStaff staff = proj.GetComponent<LongStaff>();
        if (staff != null)
        {
            staff.SetDamage(data.baseDamage);
            staff.Launch(direction, data.attackSpeed);
            return;
        }

        if (data.weaponName == "포션")
        {
            if (rb != null)
            {
                rb.gravityScale = 1.5f;
                Vector2 throwVelocity = new Vector2(direction.x * 5f, 5f);
                rb.linearVelocity = throwVelocity;
            }

            Potion potion = proj.GetComponent<Potion>();
            if (potion != null)
            {
                potion.potionType = PotionEffectHelper.GetRandomEffect();
            }
            return;
        }

        Animator weaponAnimator = proj.GetComponent<Animator>();
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("isAttack", true);
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = direction * data.attackSpeed;
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
                bell.SetDamage(data.baseDamage);
                bell.SetDirection(directionSign);
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