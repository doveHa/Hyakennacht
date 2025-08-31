using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ES : MonoBehaviour
{
    public WeaponHandler weaponHandler;
    public WeaponData startingWeapon;

    private void Start()
    {
        weaponHandler.EquipWeapon(startingWeapon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            bool isLeft = transform.localScale.x < 0;
            Debug.Log("무기 사용 시도");
            weaponHandler.UseWeapon(isLeft);
        }
    }
}
