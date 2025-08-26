using Enemy;
using UnityEngine;

public class PlayerRecognize : MonoBehaviour
{
    public bool Flag;
    private EnemyController _controller;

    void Start()
    {
        Flag = true;
        _controller = GetComponentInParent<EnemyController>();
    }
    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (Flag && collider2D.gameObject.tag.Equals("Player"))
        {
            Flag = false;
            _controller.SetTarget(collider2D.transform);
            //_controller.ChangeState(new ChasePlayerState(_controller));
            _controller.ChangeState(new AttackState(_controller));
        }
    }
}