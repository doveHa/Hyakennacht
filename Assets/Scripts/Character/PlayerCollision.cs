using System.Collections;
using Manager;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float distance;
    public GameObject GuideKey;
    private bool _isInvincible = false;
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            if (!_isInvincible)
            {
                _isInvincible = true;
                StartCoroutine(InvincibleCoroutine());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Projectile"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(collision.gameObject);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("InteractionAble"))
        {
            GuideKey.transform.position = other.transform.position + new Vector3(distance, 0, 0);
            GuideKey.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("InteractionAble"))
        {
            GuideKey.SetActive(false);
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