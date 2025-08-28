using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        public static bool IsMoving;
        public Vector2 MoveVector { get; set; } // 외부 입력 세팅
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D parentRb; // 부모 Rigidbody

        void Awake()
        {
            if (transform.parent != null)
            {
                parentRb = transform.parent.GetComponent<Rigidbody2D>();
                if (parentRb != null)
                {
                    parentRb.gravityScale = 0f;
                    parentRb.freezeRotation = true;
                    parentRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                }
                else
                {
                    Debug.LogError("부모에 Rigidbody2D가 없습니다!");
                }
            }
            else
            {
                Debug.LogError("Movement 스크립트가 부모 없이 존재합니다!");
            }
        }

        void FixedUpdate()
        {
            if (parentRb == null) return;

            if (MoveVector != Vector2.zero)
            {
                // 속도 기반 이동
                parentRb.linearVelocity = MoveVector.normalized * moveSpeed;
                IsMoving = true;
            }
            else
            {
                // 입력 없으면 정지
                parentRb.linearVelocity = Vector2.zero;
                IsMoving = false;
            }
        }
    }
}
