using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponBehavior
{
    void Initialize(WeaponData data, Transform firePoint);
    void Attack();
}

public interface IFlippableWeapon
{
    void SetFacingDirection(bool isLeft);
}

public class WeaponHandler : MonoBehaviour
{
    private Transform firePoint;
    public Transform weaponVisualHolder;
    private Transform tailFirePoint;

    private IWeaponBehavior currentBehavior;
    private MonoBehaviour currentScript;
    public GameObject currentVisual; //HR: private -> public 외부에서 접근 가능
    public WeaponData currentData; //HR: private -> public 외부에서 접근 가능

    private Transform weaponFacingProxy;

    public void Initialize(Transform firePoint, Transform weaponVisualHolder, Transform tailFirePoint)
    {
        this.firePoint = firePoint;
        this.weaponVisualHolder = weaponVisualHolder;
        this.tailFirePoint = tailFirePoint;

        var proxyGo = new GameObject("WeaponFacingProxy");
        weaponFacingProxy = proxyGo.transform;
        weaponFacingProxy.SetParent(weaponVisualHolder.parent, worldPositionStays: false);
        weaponFacingProxy.localPosition = Vector3.zero;
        weaponFacingProxy.localRotation = Quaternion.identity;
    }

    public void EquipWeapon(WeaponData data)
    {
        //HR: 유효성 검사 추가
        if (data == null)
        {
            Debug.LogWarning("EquipWeapon: 전달된 WeaponData가 유효하지 않아 장착할 수 없습니다.");
            return;
        }

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
            currentVisual = Instantiate(data.visualPrefab, weaponFacingProxy);
            currentVisual.transform.localRotation = Quaternion.identity;

            Vector3 offset = Vector3.zero;

            if (data.weaponName == "꼬리" && tailFirePoint != null)
                currentVisual.transform.localPosition = tailFirePoint.localPosition;
            else
            {
                // BonkSwinger가 있으면 위치 오프셋을 받아옴
                var swinger = currentVisual.GetComponent<Swinger>();
                if (swinger != null)
                {
                    Vector2 customOffset = swinger.GetSpawnOffset();
                    offset = new Vector3(customOffset.x, customOffset.y, 0f);
                }
            }
            currentVisual.transform.localPosition = offset;
        }
        else //HR: 디버그 메시지 추가
        {
            // visualPrefab이 null일 경우 디버그 메시지를 남기고 currentVisual을 null로 설정
            Debug.LogWarning($"EquipWeapon: {data.weaponName}의 비주얼 프리팹이 할당되지 않았습니다. 비주얼 없이 장착합니다.");
            currentVisual = null;
        }

    }

    public WeaponData GetCurrentWeaponData()
    {
        return currentData;
    }

    public void ChangeWeapon(WeaponData newData)
    {
        EquipWeapon(newData);
    }

    public void UseWeapon()
    {
        Debug.Log("어택 호출 전");
        if (currentBehavior != null)
        {
            currentBehavior.Attack();
        }
    }

    // 이 함수를 수정합니다.
    public void UpdateWeaponDirection(bool flipX)
    {
        if (weaponFacingProxy != null)
        {
            float visualDirection = flipX ? 1f : -1f;
            Vector3 scale = weaponFacingProxy.localScale;

            weaponFacingProxy.localScale = new Vector3(Mathf.Abs(scale.x) * visualDirection, scale.y, scale.z);
        }

        if (currentScript is IFlippableWeapon flippable)
            flippable.SetFacingDirection(flipX);
    }

    //HR
    /*    public void ChangeWeaponByPrefab(GameObject visualPrefab, string weaponName)
        {
            if (currentVisual != null) Destroy(currentVisual);

            currentVisual = Instantiate(visualPrefab, weaponVisualHolder);
            currentVisual.transform.localPosition = Vector3.zero;
            currentVisual.transform.localRotation = Quaternion.identity;

            Debug.Log($"WeaponHandler에 {weaponName} 비주얼 적용 완료");
        }*/

    // 현재 무기를 드랍하고 새로운 무기를 장착하는 역할을 합니다.
    public void SwapWeapon(WeaponData newWeaponData, Vector3 dropPosition)
    {
        // 1. 현재 무기(currentData)가 있으면 드랍합니다.
        if (currentData != null)
        {
            // currentData의 비주얼 프리팹을 사용하여 새 GameObject를 생성합니다.
            GameObject droppedVisual = Instantiate(currentData.visualPrefab, dropPosition, Quaternion.identity);

            // 생성된 GameObject에 WeaponPickup 스크립트를 추가하고 WeaponData를 설정합니다.
            var pickupScript = droppedVisual.AddComponent<WeaponPickup>();
            pickupScript.SetWeaponData(currentData);
            Debug.Log($"드랍된 무기: {currentData.weaponName}");
        }

        // 2. 새 무기를 장착합니다.
        EquipWeapon(newWeaponData);
    }

    // 이 메소드는 WeaponData가 아닌 visualPrefab으로 바로 장착할 때 사용합니다.
    public void ChangeWeaponByPrefab(GameObject visualPrefab, string weaponName)
    {
        if (currentVisual != null) Destroy(currentVisual);

        currentVisual = Instantiate(visualPrefab, weaponVisualHolder);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;

        Debug.Log($"WeaponHandler에 {weaponName} 비주얼 적용 완료");
    }

    public IWeaponBehavior GetCurrentBehavior() => currentBehavior;
}