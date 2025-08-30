using UnityEngine;

public class StallTrigerSimple : MonoBehaviour
{
    public int stallIndex; // 0~2
    public StartWeapon selector;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            selector.SetPlayerInRange(stallIndex, true);
            Debug.Log($"플레이어가 가판대 {stallIndex + 1} 범위 안으로 들어왔습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            selector.SetPlayerInRange(stallIndex, false);
            Debug.Log($"플레이어가 가판대 {stallIndex + 1} 범위에서 나갔습니다.");
        }
    }
}
