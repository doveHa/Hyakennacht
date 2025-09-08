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
        [SerializeField] private float rayDistance = 0.2f;

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
                float distance = moveSpeed * Time.fixedDeltaTime;
                Vector2 targetPos = rb.position + MoveVector * distance;
                rb.MovePosition(targetPos);
                
                /*

                if (hit.collider == null) // 벽 없음
                {
                    GameManager.Manager.Player.transform.position += (Vector3)MoveVector * distance;
                }
                */

                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
        }
    }
}