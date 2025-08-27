using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallAttack : MonoBehaviour, IWeaponBehavior
{
    private WeaponData data;
    private Transform firePoint;
    public int maxInstallCount = 3;

    private static List<GameObject> installedFlowers = new(); // 설치된 마법꽃 리스트

    public void Initialize(WeaponData data, Transform firePoint)
    {
        this.data = data;
        this.firePoint = firePoint;
    }

    public void Attack()
    {
        if (data.prefab == null)
        {
            Debug.LogWarning("설치형 프리팹이 없습니다.");
            return;
        }

        float playerDirection = firePoint.parent.localScale.x;

        Vector3 offset = firePoint.right * 1f * playerDirection + firePoint.up * -0.5f;
        Vector3 installPosition = firePoint.position + offset;

        GameObject installed = GameObject.Instantiate(data.prefab, installPosition, Quaternion.identity);

        if (installed.TryGetComponent<Trap>(out var trap))
        {
            trap.SetDamage(data.baseDamage);
        }
        else if (installed.TryGetComponent<MagicFlower>(out var flower))
        {
            flower.SetDamage(data.baseDamage);
            // 최대 설치 개수 제한
            installedFlowers.Add(installed);
            if (installedFlowers.Count > maxInstallCount)
            {
                GameObject oldest = installedFlowers[0];
                installedFlowers.RemoveAt(0);
                Destroy(oldest);
            }
        }

        Debug.Log("설치 완료: " + installed.name);
    }
}
