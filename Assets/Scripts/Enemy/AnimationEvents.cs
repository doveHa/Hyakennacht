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
        _controller.ChangeState(new RandomMoveState(_controller));
    }
}