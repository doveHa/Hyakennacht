using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public static bool IsMoving;
        public Vector3 MoveVector { get; set; }

        void Start()
        {
            MoveVector = Vector3.zero;
        }

        void Update()
        {
            GameManager.Manager.Player.transform.position += MoveVector * Constant.Player.MOVE_SPEED;
        }

    }
}