using Character;
using UnityEngine;

public class FacingProvider2D : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // ����(�ڵ� Ž��)
    public Dash dash;                     // ����(�ڵ� Ž��)

    // �ܺο��� �б� ���� API
    public bool IsFacingLeft()
    {
        // 1) Dash.IsLeftSight �켱
        if (!dash) dash = GetComponentInChildren<Dash>();
        if (dash) return dash.IsLeftSight;

        // 2) SpriteRenderer.flipX ���
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer) return spriteRenderer.flipX;

        // 3) ������ ������: localScale.x
        return Mathf.Sign(transform.localScale.x) < 0f;
    }

    public float FaceSign() => IsFacingLeft() ? -1f : 1f; // -1=����, +1=������
}
