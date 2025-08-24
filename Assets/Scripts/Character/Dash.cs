using System.Collections;
using Manager;
using UnityEngine;

namespace Character
{
    public class Dash : MonoBehaviour
    {
        public bool IsLeftSight { private get; set; }
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
            int currentFrame = 0;
            while (currentFrame < Constant.Roll.ROLL_FRAME)
            {
                currentFrame++;
                if (currentFrame > Constant.Roll.START_FRAME)
                {
                    if (IsLeftSight)
                    {
                        GameManager.Manager.Player.transform.position += new Vector3((-1) * Constant.Roll.ROLL_DISTANCE, 0, 0);
                    }
                    else
                    {
                        GameManager.Manager.Player.transform.position += new Vector3(Constant.Roll.ROLL_DISTANCE, 0, 0);
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
            _isDashing = false;
        }
    }
}