using Manager;
using UnityEngine;

public class SmallStraw : MonoBehaviour
{
    private Vector3 _targetPosition;
    private Rigidbody2D _rigidbody2D;

    public float cooldown = 20f;
    private float startTime;

    public float speed;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (startTime + cooldown < Time.time)
        {
            Destroy(gameObject);
        }
        else
        {
            _rigidbody2D.AddForce(
                (GameManager.Manager.PlayerScript.Target.position - transform.position).normalized * speed,
                ForceMode2D.Impulse);
        }
    }

    public void SetTarget(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag.Equals("Enemy"))
        {
        }
        else
        {
            Destroy(gameObject);

        }
    }
}