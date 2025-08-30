using Character;
using UnityEngine;

public class FacingProvider2D : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 선택(자동 탐색)
    public Dash dash;                     // 선택(자동 탐색)

    // 외부에서 읽기 쉬운 API
    public bool IsFacingLeft()
    {
        // 1) Dash.IsLeftSight 우선
        if (!dash) dash = GetComponentInChildren<Dash>();
        if (dash) return dash.IsLeftSight;

        // 2) SpriteRenderer.flipX 대안
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer) return spriteRenderer.flipX;

        // 3) 마지막 안전망: localScale.x
        return Mathf.Sign(transform.localScale.x) < 0f;
    }

    public float FaceSign() => IsFacingLeft() ? -1f : 1f; // -1=왼쪽, +1=오른쪽
}
