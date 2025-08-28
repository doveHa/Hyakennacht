using Manager;
using UnityEngine;

namespace Enemy
{
    public class HitCollider : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                SystemManager.Manager.HpControl.MinusHp();
            }
        }
    }
}