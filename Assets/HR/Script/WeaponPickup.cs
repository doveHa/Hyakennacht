using UnityEngine;
using static Constant;

public class WeaponPickup : MonoBehaviour
{
    private WeaponData weaponData;
    private bool playerInRange = false;

    public void SetWeaponData(WeaponData data)
    {
        this.weaponData = data;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Q 키를 눌러 무기를 획득하세요.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            var player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                var weaponHandler = player.GetComponentInChildren<WeaponHandler>();
                if (weaponHandler != null)
                {
                    // 먼저 weaponData가 null인지 확인합니다.
                    /*                    if (weaponData == null)
                                        {
                                            Debug.LogWarning("WeaponPickup: 무기 데이터가 유효하지 않아 무기를 주울 수 없습니다.");
                                            Destroy(gameObject); // 유효하지 않은 무기는 제거
                                            return;
                                        }*/

                    WeaponData currentEquippedWeaponData = weaponHandler.currentData; // or weaponHandler.GetCurrentWeaponData();

                    // SwapWeapon을 호출하여 현재 무기를 드랍하고 새로운 무기를 장착합니다.
                    weaponHandler.SwapWeapon(this.weaponData, player.transform.position);

                    // 드랍된 무기 오브젝트 파괴
                    Destroy(gameObject);
                }
            }
        }
    }
}
