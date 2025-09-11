using Character;
using Character.Skill;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Constant;

namespace Manager
{
    public class PlayerInputManager : AbstractManager<PlayerInputManager>
    {
        private PlayerInput _playerInput;
        [SerializeField] private SkillCaster _caster;
        private SkillBase _activeSkill1, _activeSkill2;

        protected override void Awake()
        {
            base.Awake();

            if (!_caster) _caster = FindFirstObjectByType<SkillCaster>(FindObjectsInactive.Include);

            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Movement.Move.performed += StartMove;
            _playerInput.Movement.Move.canceled += EndMove;
            _playerInput.Movement.Roll.started += StartRoll;
            _playerInput.Attack.Basic.started += StartBasicAttack;
            _playerInput.Attack.Basic.canceled += EndBasicAttack;
            _playerInput.Attack.ActiveSkill1.started += RunActiveSkill1;
            _playerInput.Attack.ActiveSkill2.started += RunActiveSkill2;
        }

/*
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameManager.Manager.SetPlayer(player);
            }
        }
        */
        private void StartMove(InputAction.CallbackContext ctx)
        {
            Movement.IsMoving = true;
            AnimationManager.Manager.StartMoveAnimation();
            Vector2 movement = ctx.ReadValue<Vector2>();

            var player = GameManager.Manager.Player;
            if (player == null) return;

            if (movement.x < 0)
                GameManager.Manager.PlayerSight(true);
            else if (movement.x > 0)
                GameManager.Manager.PlayerSight(false);

            var movementComp = player.GetComponentInChildren<Movement>();
            if (movementComp != null)
                movementComp.MoveVector = movement;
        }

        private void EndMove(InputAction.CallbackContext ctx)
        {
            Movement.IsMoving = false;
            AnimationManager.Manager.EndMoveAnimation();

            var player = GameManager.Manager.Player;
            if (player == null) return;

            var movementComp = player.GetComponentInChildren<Movement>();
            if (movementComp != null)
                movementComp.MoveVector = Vector2.zero;
        }

        /*        private void StartMove(InputAction.CallbackContext ctx)
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
        */
        private void StartRoll(InputAction.CallbackContext ctx)
        {
            var player = GameManager.Manager.Player;
            if (player != null)
                player.GetComponentInChildren<Dash>().StartDashCoroutine();
        }

        private void StartBasicAttack(InputAction.CallbackContext ctx)
        {
            Player playerScript = GameManager.Manager.PlayerScript;

            if (playerScript != null)
            {
                var behavior = playerScript.weaponHandler.GetCurrentBehavior();

                if (behavior is ChargeAttack chargeAttack)
                {
                    chargeAttack.StartCharge();
                }
                else
                {
                    playerScript.weaponHandler.UseWeapon();
                }
            }
        }

        private void EndBasicAttack(InputAction.CallbackContext ctx)
        {
            Player playerScript = GameManager.Manager.PlayerScript;

            if (playerScript != null)
            {
                var behavior = playerScript.weaponHandler.GetCurrentBehavior();

                if (behavior is ChargeAttack chargeAttack)
                {
                    chargeAttack.EndCharge();
                }
            }
        }

        private void RunActiveSkill1(InputAction.CallbackContext ctx)
        {
            Debug.Log("[Input] K pressed");
            if (_activeSkill1 == null)
            {
                var slots = _caster.GetSlots();
                if (slots != null && slots.Length > 0) _activeSkill1 = slots[0];
            }

            if (_caster && _activeSkill1) _caster.TryCast(_activeSkill1);
        }

        //public void ChangeActiveSkill1(SkillBase skill) => _activeSkill1 = skill;
        //public void ChangeActiveSkill2(SkillBase skill) => _activeSkill2 = skill;

        public void ChangeActiveSkill1(SkillBase skill)
        {
            _playerInput.Attack.ActiveSkill1.started -= RunActiveSkill1;
            _activeSkill1 = skill;
            _playerInput.Attack.ActiveSkill1.started += RunActiveSkill1;
            //_activeSkill1 = skill;
        }

        private void RunActiveSkill2(InputAction.CallbackContext ctx)
        {
            Debug.Log("[Input] L pressed");
            if (_activeSkill2 == null)
            {
                var slots = _caster.GetSlots();
                if (slots != null && slots.Length > 1) _activeSkill2 = slots[1];
            }

            if (_caster && _activeSkill2) _caster.TryCast(_activeSkill2);
        }

        public void ChangeActiveSkill2(SkillBase skill)
        {
            _playerInput.Attack.ActiveSkill2.started -= RunActiveSkill2;
            _activeSkill2 = skill;
            _playerInput.Attack.ActiveSkill2.started += RunActiveSkill2;
        }


        //HR: 씬 전환을 위해 추가
        public void DisableInput()
        {
            _playerInput?.Disable();
        }

        protected void OnDestroy()
        {
            if (_playerInput != null)
            {
                _playerInput.Attack.ActiveSkill1.started -= RunActiveSkill1;
                _playerInput.Attack.ActiveSkill2.started -= RunActiveSkill2;
                _playerInput.Disable();
                _playerInput.Dispose();
                _playerInput = null;
            }
        }
    }
}