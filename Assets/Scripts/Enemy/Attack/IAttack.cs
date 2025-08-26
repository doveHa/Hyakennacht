using UnityEngine;

namespace Enemy.Attack
{
    public abstract class IAttack : MonoBehaviour
    {
        public abstract void Attack(Vector3 targetPosition);

        public virtual void Exit()
        {
            GetComponentInChildren<PlayerRecognize>().Flag = true;
        }
        
    }
}