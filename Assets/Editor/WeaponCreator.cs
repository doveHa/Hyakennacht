using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponCreator : EditorWindow
{
    private string weaponName = "New Weapon";
    private WeaponFaction faction = WeaponFaction.Yokai;
    private WeaponGrade grade = WeaponGrade.Medium;
    private WeaponCategory category = WeaponCategory.Melee;
    private WeaponBehaviorType behaviorType = WeaponBehaviorType.Bonk;

    private int baseDamage = 10;
    private float attackSpeed = 1.0f;
    private Sprite icon;
    private GameObject prefab;
    private GameObject visualPrefab; // visualPrefab 변수 추가

    [MenuItem("Tools/Weapon Creator")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreator>("Weapon Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("무기 정보", EditorStyles.boldLabel);

        weaponName = EditorGUILayout.TextField("무기 이름", weaponName);
        faction = (WeaponFaction)EditorGUILayout.EnumPopup("진영", faction);
        grade = (WeaponGrade)EditorGUILayout.EnumPopup("등급", grade);
        category = (WeaponCategory)EditorGUILayout.EnumPopup("카테고리", category);
        behaviorType = (WeaponBehaviorType)EditorGUILayout.EnumPopup("공격 방식", behaviorType);

        baseDamage = EditorGUILayout.IntField("기본 공격력", baseDamage);
        attackSpeed = EditorGUILayout.FloatField("공격 속도", attackSpeed);

        icon = (Sprite)EditorGUILayout.ObjectField("아이콘", icon, typeof(Sprite), false);
        prefab = (GameObject)EditorGUILayout.ObjectField("프리팹", prefab, typeof(GameObject), false);
        visualPrefab = (GameObject)EditorGUILayout.ObjectField("비주얼 프리팹", visualPrefab, typeof(GameObject), false); // visualPrefab 필드 추가

        if (GUILayout.Button("무기 생성"))
        {
            CreateWeaponAsset();
        }
    }

    void CreateWeaponAsset()
    {
        WeaponData newWeapon = ScriptableObject.CreateInstance<WeaponData>();

        newWeapon.weaponName = weaponName;
        newWeapon.faction = faction;
        newWeapon.grade = grade;
        newWeapon.category = category;
        newWeapon.behaviorType = behaviorType;
        newWeapon.baseDamage = baseDamage;
        newWeapon.attackSpeed = attackSpeed;
        newWeapon.weaponIcon = icon;
        newWeapon.prefab = prefab;
        newWeapon.visualPrefab = visualPrefab; // visualPrefab 할당 추가

        string path = $"Assets/Weapons/{weaponName}.asset";
        AssetDatabase.CreateAsset(newWeapon, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newWeapon;

        Debug.Log($"무기 생성 완료: {path}");
    }
}