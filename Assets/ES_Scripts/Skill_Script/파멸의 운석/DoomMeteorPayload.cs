using UnityEngine;

/// �ĸ��� � ���� ���̷ε�: ���� ������ ��� ����, ��.
public class DoomMeteorPayload : MonoBehaviour, IFallPayload
{
    [Header("Impact Damage")]
    public int impactDamage = 80;   // SO���� ���� ����

    public void OnImpact(Transform owner, Vector2 hitPoint, Collider2D hit)
    {
        var enemy = hit.GetComponentInParent<EnemyStats>();
        if (enemy && impactDamage > 0)
            enemy.TakeDamage(impactDamage);
        // �߰� ����Ʈ/�˹� �� ��. FallingProjectile2D�� �ٷ� ReturnToPool ó��.
    }
}
