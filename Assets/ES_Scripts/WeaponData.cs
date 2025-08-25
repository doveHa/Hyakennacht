using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponFaction { Yokai, Witch }
public enum WeaponGrade { High, Medium, Low }
public enum WeaponCategory { Melee, Ranged, Magic, Install }
public enum WeaponBehaviorType { Bonk, Projectile, Charge, Install }

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponFaction faction;
    public WeaponGrade grade;
    public WeaponCategory category;

    public int baseDamage;
    public float attackSpeed;
    public WeaponBehaviorType behaviorType;

    public Sprite weaponIcon;
    public GameObject prefab;
    public GameObject visualPrefab;
}

