using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Stunnable : MonoBehaviour
{
    [Header("연출 옵션")]
    public Color stunnedTint = new Color(1f, 1f, 1f, 0.5f); // 스턴 시 비주얼
    public bool debugLog = false;

    // 상태
    public bool IsStunned { get; private set; }
    private float originalSpeed;
    private SpriteRenderer[] srs;
    private Color[] origColors;

    private Coroutine stunRoutine;

    void Awake()
    {
        srs = GetComponentsInChildren<SpriteRenderer>(true);
        origColors = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) origColors[i] = srs[i].color;
    }

    public void ApplyStun(float duration)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(CoStun(duration));
    }

    private IEnumerator CoStun(float sec)
    {
        IsStunned = true;

        /*
        var enemyMove = GetComponent<EnemyMovement>(); 
        if (enemyMove != null)
        {
            originalSpeed = enemyMove.moveSpeed;
            enemyMove.moveSpeed = 0f;
        }
        */
        var enemyMove = GetComponent<Enemy_ES>();
        if (enemyMove != null)
        {
            originalSpeed = enemyMove.moveSpeed;
            enemyMove.moveSpeed = 0f;
        }

        for (int i = 0; i < srs.Length; i++)
            if (srs[i]) srs[i].color = stunnedTint;

        if (debugLog) Debug.Log($"{name} 스턴 시작 ({sec:F1}초)");

        yield return new WaitForSeconds(sec);

        // 원래 속도로 복원
        if (enemyMove != null) enemyMove.moveSpeed = originalSpeed;

        // 색상 복구
        for (int i = 0; i < srs.Length; i++)
            if (srs[i]) srs[i].color = origColors[i];

        if (debugLog) Debug.Log($"{name} 스턴 종료");

        IsStunned = false;
        stunRoutine = null;
    }
}
