using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponBehavior
{
}

public class WeaponHandler : MonoBehaviour
{
    public Transform firePoint;
    private IWeaponBehavior currentBehavior;
    private MonoBehaviour currentScript;


    public void UseWeapon()
    {
        Debug.Log("BasicAttack");
    }
}