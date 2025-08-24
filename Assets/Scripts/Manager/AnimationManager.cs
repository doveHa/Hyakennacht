using UnityEngine;

namespace Manager
{
    public class AnimationManager : AbstractManager<AnimationManager>
    {
        private Animator _animator;
        public bool RollFlag { private get; set; }

        protected override void Awake()
        {
            base.Awake();
            RollFlag = true;
        }

        void Start()
        {
            _animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();
        }

        public void StartMoveAnimation()
        {
            _animator.SetBool("IsWalk",true);
        }

        public void EndMoveAnimation()
        {
            _animator.SetBool("IsWalk",false);
        }

        public bool StartRollAnimation()
        {
            if (RollFlag)
            {
                _animator.SetTrigger("Roll");
                return true;
            }

            return false;
        }
    }
}