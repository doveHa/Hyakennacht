using Enemy;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private EnemyController _controller;

    void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    public void EndAttack()
    {
        if (_controller.Target != null)
        {
            _controller.ChangeState(new ChasePlayerState(_controller));
        }
        else
        {
            _controller.ChangeState(new RandomMoveState(_controller));
        }
    }
}