using Enemy;
using Manager;
using UnityEngine;

public class PlayerRecognize : MonoBehaviour
{
    public bool Flag;
    private EnemyController _controller;
    public float cooldown = 2f; // 쿨타임 (초)
    private float _lastAttackTime = -Mathf.Infinity;

    void Start()
    {
        Flag = true;
        _controller = GetComponentInParent<EnemyController>();
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.tag.Equals("Player") && TryAttack())
        {
            _controller.SetTarget(GameManager.Manager.PlayerScript.Target);
            //_controller.ChangeState(new ChasePlayerState(_controller));
            _controller.ChangeState(new AttackState(_controller));
        }
    }

    public bool TryAttack()
    {
        if (Time.time >= _lastAttackTime + cooldown)
        {
            _lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
}