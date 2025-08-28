using UnityEngine;

namespace Manager
{
    public class AnimationManager : AbstractManager<AnimationManager>
    {
        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();

            //HR
/*            _animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();

            if (_animator == null)
                Debug.LogError("Player Animator not found!");*/
        }

        void Start()
        {
            //_animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();
            /*            if (GameManager.Manager.Player != null)
                        {
                            _animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();
                        }

                        if (_animator == null)
                            Debug.LogError("Player Animator not found!");*/
            if (GameManager.Manager.Player == null)
            {
                Debug.LogWarning("Player not ready yet. Will retry...");
                return;
            }

            _animator = GameManager.Manager.Player.GetComponentInChildren<Animator>();

            if (_animator == null)
                Debug.LogError("Player Animator not found!");
        }

        public void StartMoveAnimation()
        {
            _animator.SetBool("IsWalk", true);
        }

        public void EndMoveAnimation()
        {
            _animator.SetBool("IsWalk", false);
        }

        public void StartDashAnimation()
        {
            _animator.SetTrigger("Dash");
        }
    }
}