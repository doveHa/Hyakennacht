using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponBehavior
{
    void Initialize(WeaponData data, Transform firePoint);
    void Attack();
}

public class WeaponHandler : MonoBehaviour
{
    private Transform firePoint;
    public Transform weaponVisualHolder;
    private Transform tailFirePoint;

    private IWeaponBehavior currentBehavior;
    private MonoBehaviour currentScript;
    private GameObject currentVisual;
    private WeaponData currentData;

    public void Initialize(Transform firePoint, Transform weaponVisualHolder, Transform tailFirePoint)
    {
        this.firePoint = firePoint;
        this.weaponVisualHolder = weaponVisualHolder;
        this.tailFirePoint = tailFirePoint;
    }

    public void EquipWeapon(WeaponData data)
    {
        if (currentScript != null)
            Destroy(currentScript);

        currentScript = data.behaviorType switch
        {
            WeaponBehaviorType.Bonk => gameObject.AddComponent<BonkAttack>(),
            WeaponBehaviorType.Projectile => gameObject.AddComponent<ProjectileAttack>(),
            WeaponBehaviorType.Charge => gameObject.AddComponent<ChargeAttack>(),
            WeaponBehaviorType.Install => gameObject.AddComponent<InstallAttack>(),
            _ => null
        };

        currentBehavior = currentScript as IWeaponBehavior;
        currentData = data;
        currentBehavior?.Initialize(data, firePoint);

        if (currentVisual != null)
            Destroy(currentVisual);

        if (data.visualPrefab != null && weaponVisualHolder != null)
        {
            currentVisual = Instantiate(data.visualPrefab, weaponVisualHolder);
            currentVisual.transform.localRotation = Quaternion.identity;

            if (data.weaponName == "꼬리" && tailFirePoint != null)
                currentVisual.transform.localPosition = tailFirePoint.localPosition;
            else
                currentVisual.transform.localPosition = Vector3.zero;
        }
    }

    public WeaponData GetCurrentWeaponData()
    {
        // 현재 무기 데이터
        return currentData;
    }

    public void ChangeWeapon(WeaponData newData)
    {
        // 무기 바꾸기
        EquipWeapon(newData);
    }

    public void UseWeapon()
    {
        Debug.Log("어택 호출 전");
        currentBehavior?.Attack();
    }
}
