using UnityEngine;

public static class Constant
{
    public static class Player
    {
        public static float MOVE_SPEED = 0.025f;
    }

    public static class Enemy
    {
        public static float MOVE_SPEED = 1.5f;
    }

    public static class SpawnEnemy
    {
        public static int MIN_ENEMIES = 3;
        public static int MAX_ENEMIES = 5;
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
        public static float ROLL_DISTANCE = 0.025f;
    }

    public static class Scene
    {
        public static string WITCHLOBBY = "WitchLobbyScene(Temp)";
        public static string YOKAILOBBY = "YokaiLobbyScene(Temp)";
    }
}