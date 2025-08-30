using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement_HR : MonoBehaviour
    {
        public static bool IsMoving;
        public Vector2 MoveVector { get; set; } // 외부 입력 세팅
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D rb;
        private Transform parentTransform;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            parentTransform = transform.parent;
        }

        void FixedUpdate()
        {
            //Debug.Log($"MoveVector: {MoveVector}, IsMoving: {IsMoving}");

            if (MoveVector != Vector2.zero)
            {
                Vector2 targetPos = rb.position + MoveVector * moveSpeed * Time.fixedDeltaTime;

                // 벽 체크 (Raycast)
                RaycastHit2D hit = Physics2D.Raycast(rb.position, MoveVector, moveSpeed * Time.fixedDeltaTime);
                if (hit.collider == null) // 벽 없음
                {
                    rb.MovePosition(targetPos);

                    // 부모도 자식 이동만큼 같이 이동
                    if (parentTransform != null)
                        parentTransform.position += (Vector3)(MoveVector * moveSpeed * Time.fixedDeltaTime);
                }

                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
        }

        /*        void Start()
                {
                    MoveVector = Vector3.zero;
                }

                void Update()
                {
                    GameManager.Manager.Player.transform.position += MoveVector * Constant.Player.MOVE_SPEED;
                }*/
    }
}