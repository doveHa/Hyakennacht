using Character;
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
            _playerInput.Attack.Basic.started += StartBasicAttack;
            _playerInput.Attack.ActiveSkill1.started += RunActiveSkill1;
            _playerInput.Attack.ActiveSkill2.started += RunActiveSkill2;
        }


        private void StartMove(InputAction.CallbackContext ctx)
        {
            Movement.IsMoving = true;
            AnimationManager.Manager.StartMoveAnimation();
            Vector2 movement = ctx.ReadValue<Vector2>();

            if (movement.x < 0)
            {
                GameManager.Manager.PlayerSight(true);
            }
            else if (movement.x > 0)
            {
                GameManager.Manager.PlayerSight(false);
            }

            GameManager.Manager.SetMoveVector(movement);
            //GameManager.Manager.MovePlayer(movement * GameObject.Find("Coff").GetComponent<CoffTest>().MoveSpeedCoff);
        }

        private void EndMove(InputAction.CallbackContext ctx)
        {
            Movement.IsMoving = false;
            AnimationManager.Manager.EndMoveAnimation();
            GameManager.Manager.SetMoveVector(Vector2.zero);
        }

        private void StartRoll(InputAction.CallbackContext ctx)
        {
            GameManager.Manager.Player.GetComponentInChildren<Dash>().StartDashCoroutine();
        }

        private void StartBasicAttack(InputAction.CallbackContext ctx)
        {
            GameManager.Manager.PlayerScript.WeaponHandler.UseWeapon();
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
    }
}