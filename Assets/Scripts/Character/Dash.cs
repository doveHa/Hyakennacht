using System.Collections;
using Manager;
using UnityEngine;

namespace Character
{
    public class Dash : MonoBehaviour
    {
        public bool IsLeftSight { get; set; }
        private bool _isDashing;

        public void StartDashCoroutine()
        {
            if (!_isDashing)
            {
                AnimationManager.Manager.StartDashAnimation();
                StartCoroutine(StartDash());
            }
        }


        private IEnumerator StartDash()
        {
            SystemManager.Manager.HpControl.SetInvincible(true);
            Vector2 moveVector = GameManager.Manager.Player.transform.GetComponentInChildren<Movement>().MoveVector;

            int currentFrame = 0;
            while (currentFrame < Constant.Roll.ROLL_FRAME)
            {
                currentFrame++;
                if (currentFrame > Constant.Roll.START_FRAME)
                {
                    if (moveVector == Vector2.zero)
                    {
                        if (IsLeftSight)
                        {
                            GameManager.Manager.Player.transform.position +=
                                new Vector3((-1) * Constant.Roll.ROLL_DISTANCE, 0, 0);
                        }
                        else
                        {
                            GameManager.Manager.Player.transform.position +=
                                new Vector3(Constant.Roll.ROLL_DISTANCE, 0, 0);
                        }
                    }
                    else
                    {
                        GameManager.Manager.Player.transform.position +=
                            (Vector3)GameManager.Manager.Player.transform.GetComponentInChildren<Movement>()
                                .MoveVector *
                            Constant.Roll.ROLL_DISTANCE;
                    }
                }

                yield return null;
            }
        }

        public void LockDash()
        {
            _isDashing = true;
        }

        public void UnlockDash()
        {
            SystemManager.Manager.HpControl.SetInvincible(false);
            _isDashing = false;
        }
    }
}