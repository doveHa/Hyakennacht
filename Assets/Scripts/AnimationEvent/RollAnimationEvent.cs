using Character;
using Manager;
using UnityEngine;

namespace AnimationEvent
{
    public class RollAnimationEvent : MonoBehaviour
    {
        public void StartRoll()
        {
            AnimationManager.Manager.RollFlag = false;
            SystemManager.Manager.HpControl.SetInvincible(true);
        }

        public void StopRoll()
        {
            SystemManager.Manager.HpControl.SetInvincible(false);
            Constant.Player.MOVE_SPEED /= 2;
            AnimationManager.Manager.RollFlag = true;
        }
    }
}