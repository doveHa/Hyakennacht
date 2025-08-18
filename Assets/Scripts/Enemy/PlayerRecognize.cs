using Enemy;
using UnityEngine;

public class PlayerRecognize : MonoBehaviour
{
    private EnemyController _controller;

    void Start()
    {
        _controller = GetComponentInParent<EnemyController>();
    }
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag.Equals("Player"))
        {
            _controller.SetTarget(collider2D.transform);
            _controller.ChangeState(new ChasePlayerState(_controller));
            Destroy(this);
        }
    }
}