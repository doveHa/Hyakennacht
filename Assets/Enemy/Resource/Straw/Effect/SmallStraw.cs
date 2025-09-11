using Manager;
using UnityEngine;

public class SmallStraw : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    
    public float speed;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rigidbody2D.AddForce(
            (GameManager.Manager.PlayerScript.Target.position - transform.position).normalized * speed,
            ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }
    }
}