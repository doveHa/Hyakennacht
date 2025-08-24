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
            SystemManager.Manager.hpControl.SetInvincible(true);
        }

        public void StopRoll()
        {
            AnimationManager.Manager.RollFlag = true;
            SystemManager.Manager.hpControl.SetInvincible(false);
            Constant.Player.MOVE_SPEED /= 2;
        }
    }
}