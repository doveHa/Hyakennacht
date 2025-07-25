using Character.Skill;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class PlayerInputManager : AbstractManager<PlayerInputManager>
    {
        private PlayerInput _playerInput;
        private SkillBase _activeSkill1, _activeSkill2;

        protected override void Awake()
        {
            base.Awake();

            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Movement.Move.performed += StartMove;
            _playerInput.Movement.Move.canceled += EndMove;
            _playerInput.Movement.Roll.started += StartRoll;
            _playerInput.Attack.ActiveSkill1.started += RunActiveSkill1;
            _playerInput.Attack.ActiveSkill2.started += RunActiveSkill2;
        }


        private void StartMove(InputAction.CallbackContext ctx)
        {
            AnimationManager.Manager.StartMoveAnimation();
            Vector2 movement = ctx.ReadValue<Vector2>();

            if (movement.x < 0)
            {
                FlipX(true);
            }
            else if (movement.x > 0)
            {
                FlipX(false);
            }
            
            //GameManager.Manager.MovePlayer(movement * Constant.SPEED.MOVESPEED);
            GameManager.Manager.MovePlayer(movement * GameObject.Find("Coff").GetComponent<CoffTest>().MoveSpeedCoff);
        }

        private void EndMove(InputAction.CallbackContext ctx)
        {
            AnimationManager.Manager.EndMoveAnimation();
            GameManager.Manager.MovePlayer(Vector2.zero);
        }

        private void StartRoll(InputAction.CallbackContext ctx)
        {
        }

        private void StartBasicAttack(InputAction.CallbackContext ctx)
        {
        }

        private void RunActiveSkill1(InputAction.CallbackContext ctx)
        {
            if (_activeSkill1 != null)
            {
                _activeSkill1.Run();
            }
        }

        public void ChangeActiveSkill1(SkillBase skill)
        {
            _activeSkill1 = skill;
        }

        private void RunActiveSkill2(InputAction.CallbackContext ctx)
        {
            if (_activeSkill2 != null)
            {
                _activeSkill2.Run();
            }
        }

        public void ChangeActiveSkill2(SkillBase skill)
        {
            _activeSkill2 = skill;
        }

        private void FlipX(bool flipX)
        {
            if (flipX)
            {
                GameManager.Manager.Player.transform.rotation = Constant.FLIP.NOTFLIPPED;
            }
            else
            {
                GameManager.Manager.Player.transform.rotation = Constant.FLIP.FLIPPED;
            }
        }
    }
}