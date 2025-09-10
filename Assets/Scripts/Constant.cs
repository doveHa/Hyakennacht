using System.Collections.Generic;
using UnityEngine;

public static class Constant
{
    public static class Player
    {
        public static float MOVE_SPEED = 0.05f;
    }

    public static class Enemy
    {
        public static float MOVE_SPEED = 1.5f;
    }

    public static class SpawnEnemy
    {
        public static int MIN_ENEMIES = 3;
        public static int MAX_ENEMIES = 6;
        public static float SPAWN_DUPLICATION_DISTANCE = 0.5f;
    }

    public static class Flip
    {
        public static Quaternion NOTFLIPPED = Quaternion.Euler(0, 0, 0);
        public static Quaternion FLIPPED = Quaternion.Euler(0, 180, 0);
    }

    public static class Roll
    {
        public static int ROLL_FRAME = 20;
        public static int START_FRAME = 4;
        public static float ROLL_DISTANCE = 0.125f;
    }

    public static class Scene
    {
        public static string WITCHLOBBY = "WitchLobbyScene(Temp)";
        public static string YOKAILOBBY = "YokaiLobbyScene(Temp)";
    }

    public static class Stage
    {
        public static int MIDDLE_BOSS = 5;
        public static int FINAL_BOSS = 10;
    }

    public static string[] PREFAB_PATHS =
    {
        "Assets/Prefab/DownStair.prefab",
        "Assets/Prefab/UpStair.prefab",
        "Assets/Enemy/Prefab/Boss1.prefab",
        "Assets/Enemy/Prefab/Boss2.prefab",
        "Assets/Enemy/Prefab/Ghost.prefab",
        "Assets/Enemy/Prefab/Golem.prefab",
        "Assets/Enemy/Prefab/Kappa.prefab",
        "Assets/Enemy/Prefab/MiddleBoss.prefab",
        "Assets/Enemy/Prefab/Slime.prefab",
        "Assets/Enemy/Prefab/Straw.prefab",
        "Assets/Enemy/Prefab/Will-o-Wisp.prefab",
        "Assets/Enemy/Prefab/WitchBoss.prefab"
    };

    public static class EnemyName
    {
        public static string[] Yokaimap =
        {
            "Ghost", "Slime", "Golem"
        };

        public static string[] Witchmap =
        {
            "Will-o-Wisp", "Kappa", "Straw"
        };

        public static string YOKAI_MIDDLE_BOSS = "Boss2";
        public static string YOKAI_FINAL_BOSS = "WitchBoss";
        public static string WITCH_MIDDLE_BOSS = "Boss1";
        public static string WITCH_FINAL_BOSS = "MiddleBoss";
    }
}