using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private IEnemyState _currentState;
        public EnemySpawner Spawner { get; set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Target { get; private set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            ChangeState(new RandomMoveState(this));
        }

        void Update()
        {
            _currentState?.Update();
        }

        public void ChangeState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void SetTarget(Transform target)
        {
            Target = target;
            ChangeState(new ChasePlayerState(this));
        }

        public void ClearTarget()
        {
            Target = null;
            ChangeState(new RandomMoveState(this));
        }
        
    }
}