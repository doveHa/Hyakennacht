using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public static float MoveSpeed;
        public static bool IsMoving;
        public Vector3 MoveVector { get; set; }

        void Start()
        {
            MoveSpeed = 0.01f;
            MoveVector = Vector3.zero;
        }

        void Update()
        {
            GameManager.Manager.Player.transform.position += MoveVector * MoveSpeed;
        }

    }
}