using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        private IEnemyState _currentState;
        public EnemySpawner Spawner { get; set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Transform Target { get; private set; }

        void Awake()
        {
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            ChangeState(new RandomMoveState(this));
        }

        void Update()
        {
            Debug.Log(_currentState.GetType().Name);
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