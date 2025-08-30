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

        if (data.weaponName == "����")
        {
            soundWave = Resources.Load<GameObject>("Effects/�Ҹ� �ĵ�");
            if (soundWave == null)
                Debug.LogWarning("SoundWave �������� ã�� �� �����ϴ�.");
        }
    }

    public void Attack()
    {
        Debug.Log("���� ȣ�� �Ϸ�");

        if (data.prefab == null)
        {
            Debug.LogWarning("���� �������� �����ϴ�.");
            return;
        }

        Vector2 direction = new Vector2(firePoint.parent.localScale.x, 0).normalized;

        Vector3 Pos = firePoint.position;
        if (data.weaponName == "�� ������")
        {
            Pos += Vector3.up * 1f;
        }

        if (data.weaponName == "����" && soundWave != null)
        {
            Vector3 spawnPos = new Vector3(firePoint.position.x, firePoint.position.y + 0.5f, firePoint.position.z);
            GameObject wave = GameObject.Instantiate(soundWave, spawnPos, Quaternion.identity);
            Destroy(wave, 1f);
            //CameraShake();
        }

        GameObject proj = GameObject.Instantiate(data.prefab, Pos, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        // ��������Ʈ �������� ã�� �¿� ����
        SpriteRenderer projSprite = proj.GetComponentInChildren<SpriteRenderer>();
        if (projSprite != null)
        {
            // �÷��̾��� x �������� ������ ��������Ʈ�� ������
            projSprite.flipX = firePoint.parent.localScale.x < 0;
        }

        LongStaff staff = proj.GetComponent<LongStaff>();
        if (staff != null)
        {
            staff.SetDamage(data.baseDamage);
            staff.Launch(direction, data.attackSpeed);
            return;
        }

        if (data.weaponName == "����")
        {
            rb = proj.GetComponent<Rigidbody2D>();
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

        // �ִϸ��̼� ó��
        Animator weaponAnimator = proj.GetComponent<Animator>();
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("isAttack", true);
        }

        // ���� �������� �߻�
        
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
        Debug.Log("��鸲 ȿ�� �߻�");
    }

}
