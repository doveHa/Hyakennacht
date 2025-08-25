using System.Collections;
using UnityEngine;

public class ChargeAttack : MonoBehaviour, IWeaponBehavior
{
    private WeaponData data;
    private Transform firePoint;
    private Animator animator;

    private bool isCharging = false;
    private float chargeTime = 0f;
    public float chargeThreshold = 1.5f;

    private GameObject currentProjectile;

    public void Initialize(WeaponData data, Transform firePoint)
    {
        this.data = data;
        this.firePoint = firePoint;
        animator = GetComponentInChildren<Animator>();
    }

    public void Attack()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isCharging = true;
            chargeTime = 0f;

            if (data.weaponName == "������")
            {
                SpawnMagicCircle();
            }

            if (animator != null)
                animator.SetTrigger("Charge");
        }

        if (isCharging && Input.GetKey(KeyCode.Z))
        {
            chargeTime += Time.deltaTime;

            if (data.weaponName == "������" && currentProjectile != null)
            {
                float scale = Mathf.Lerp(1f, 2.5f, chargeTime / chargeThreshold);
                currentProjectile.transform.localScale = new Vector3(scale, scale, 1);

                Animator projAnim = currentProjectile.GetComponent<Animator>();
                if (projAnim != null)
                {
                    if (chargeTime >= chargeThreshold * 0.66f)
                        projAnim.Play("Stage3"); 
                    else if (chargeTime >= chargeThreshold * 0.33f)
                        projAnim.Play("Stage2");
                    else
                        projAnim.Play("Stage1"); 
                }
            }
        }

        if (isCharging && Input.GetKeyUp(KeyCode.Z))
        {
            isCharging = false;

            if (data.weaponName == "������")
            {
                Destroy(currentProjectile, 0.2f); 
            }
            else
            {
                bool fullyCharged = chargeTime >= chargeThreshold;
                FireProjectile(fullyCharged);
            }

            if (animator != null)
                animator.SetTrigger("Shoot");
        }
    }

    private void FireProjectile(bool fullyCharged)
    {
        if (data.prefab == null || firePoint == null) return;

        currentProjectile = Instantiate(data.prefab, firePoint.position, Quaternion.identity);

        float xDir = Mathf.Sign(firePoint.parent.localScale.x);

        Rigidbody2D rb = currentProjectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(xDir * data.attackSpeed, 0f);
        }

        SpriteRenderer sr = currentProjectile.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = xDir < 0;
        }

        Projectile p = currentProjectile.GetComponent<Projectile>();
        if (p != null)
        {
            int dmg = fullyCharged ? data.baseDamage * 2 : data.baseDamage;
            p.SetDamage(dmg);
        }
    }

    private void SpawnMagicCircle()
    {
        if (data.prefab == null || firePoint == null) return;

        Vector3 spawnPos = firePoint.position;
        spawnPos.y -= 0.5f; 

        currentProjectile = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        MagicCircle circle = currentProjectile.GetComponent<MagicCircle>();
        if (circle != null)
        {
            circle.SetDamage(data.baseDamage);
        }
    }
}
