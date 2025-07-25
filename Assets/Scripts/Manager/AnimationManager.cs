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
        }

        public void StartMoveAnimation()
        {
            _animator.SetFloat("RunState",0.3f);
        }

        public void EndMoveAnimation()
        {
            _animator.SetFloat("RunState",0f);
        }

        public void StartRollAnimation()
        {
            
        }
    }
}