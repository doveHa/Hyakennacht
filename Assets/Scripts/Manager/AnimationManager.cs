using UnityEngine;

namespace Manager
{
    public class AnimationManager : AbstractManager<AnimationManager>
    {
        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            _animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();
            Debug.Log(_animator);

        }

        public void StartMoveAnimation()
        {
            Debug.Log(_animator);

            _animator.SetBool("IsWalk", true);
        }

        public void EndMoveAnimation()
        {
            Debug.Log(_animator);

            _animator.SetBool("IsWalk", false);
        }

        public void StartDashAnimation()
        {
            _animator.SetTrigger("Dash");
        }
    }
}