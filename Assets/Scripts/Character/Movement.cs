using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public static bool IsMoving;
        public Vector2 MoveVector { get; set; } // 외부 입력 세팅
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D rb;
        //private Transform parentTransform;

        void Awake()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            //parentTransform = transform.parent;
        }

        void FixedUpdate()
        {
            if (MoveVector != Vector2.zero)
            {
                Vector2 targetPos = rb.position + MoveVector * moveSpeed * Time.fixedDeltaTime;
                // 벽 체크 (Raycast)
                rb.MovePosition(targetPos);
                /*
                RaycastHit2D hit = Physics2D.Raycast(rb.position, MoveVector, moveSpeed * Time.fixedDeltaTime);

                if (hit.collider == null) // 벽 없음
                {
                }*/

                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
        }
    }
}