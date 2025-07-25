using UnityEngine;

public static class Constant
{
    public static class FLIP
    {
        public static Quaternion NOTFLIPPED = Quaternion.Euler(0, 0, 0);
        public static Quaternion FLIPPED = Quaternion.Euler(0, 180, 0);
    }

    public static class SPEED_DISTANCE
    {
        public static float MOVESPEED = 0.01f;
        public static float ROLL_DISTANCE = 1f;
    }
}