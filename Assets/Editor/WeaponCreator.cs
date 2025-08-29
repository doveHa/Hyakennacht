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
    private GameObject visualPrefab; // visualPrefab ���� �߰�

    [MenuItem("Tools/Weapon Creator")]
    public static void ShowWindow()
    {
        GetWindow<WeaponCreator>("Weapon Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("���� ����", EditorStyles.boldLabel);

        weaponName = EditorGUILayout.TextField("���� �̸�", weaponName);
        faction = (WeaponFaction)EditorGUILayout.EnumPopup("����", faction);
        grade = (WeaponGrade)EditorGUILayout.EnumPopup("���", grade);
        category = (WeaponCategory)EditorGUILayout.EnumPopup("ī�װ�", category);
        behaviorType = (WeaponBehaviorType)EditorGUILayout.EnumPopup("���� ���", behaviorType);

        baseDamage = EditorGUILayout.IntField("�⺻ ���ݷ�", baseDamage);
        attackSpeed = EditorGUILayout.FloatField("���� �ӵ�", attackSpeed);

        icon = (Sprite)EditorGUILayout.ObjectField("������", icon, typeof(Sprite), false);
        prefab = (GameObject)EditorGUILayout.ObjectField("������", prefab, typeof(GameObject), false);
        visualPrefab = (GameObject)EditorGUILayout.ObjectField("���־� ������", visualPrefab, typeof(GameObject), false); // visualPrefab �ʵ� �߰�

        if (GUILayout.Button("���� ����"))
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
        newWeapon.visualPrefab = visualPrefab; // visualPrefab �Ҵ� �߰�

        string path = $"Assets/Weapons/{weaponName}.asset";
        AssetDatabase.CreateAsset(newWeapon, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newWeapon;

        Debug.Log($"���� ���� �Ϸ�: {path}");
    }
}