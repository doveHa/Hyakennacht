using System.Collections;
using Manager;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool _isInvincible = false;
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            Debug.Log("Enemy");
            SystemManager.Manager.HpControl.MinusHp();
            if (!_isInvincible)
            {
                _isInvincible = true;
                StartCoroutine(InvincibleCoroutine());
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        SystemManager.Manager.HpControl.SetInvincible(true);
        yield return new WaitForSeconds(1);
        SystemManager.Manager.HpControl.SetInvincible(false);
        _isInvincible = false;
    }
}
