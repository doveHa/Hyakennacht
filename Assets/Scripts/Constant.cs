using UnityEngine;

public static class Constant
{
    public static class FLIP
    {
        public static Quaternion NOTFLIPPED = Quaternion.Euler(0, 0, 0);
        public static Quaternion FLIPPED = Quaternion.Euler(0, 180, 0);
    }

    public static class ROLL
    {
        public static int ROLL_FRAME = 20;
        public static int START_FRAME = 4;
        public static float ROLL_DISTANCE = 0.025f;
        public static float DISTANCE_COFF = 20f;

    }
}