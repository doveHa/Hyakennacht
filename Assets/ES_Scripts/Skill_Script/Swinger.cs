using UnityEngine;
using System.Collections;

public class Swinger : MonoBehaviour
{
    public float swingAngle = -90f;
    public float swingDuration = 0.2f;
    public bool invertDirection = false;

    private Quaternion originalRotation;
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }

    public void Swing()
    {
        StopAllCoroutines();
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        float elapsed = 0f;
        float dir = invertDirection ? -1f : 1f;
        while (elapsed < swingDuration)
        {
            float t = elapsed / swingDuration;
            float angle = Mathf.Lerp(0, swingAngle * dir, t);
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
    }

    public Vector2 GetSpawnOffset()
    {
        return spawnOffset;
    }
}
