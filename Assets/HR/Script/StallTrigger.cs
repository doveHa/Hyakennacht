using UnityEngine;

public class StallTrigger : MonoBehaviour
{
    public int stallIndex;
    public ShopManager shopManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            shopManager.SetPlayerInRange(stallIndex, true);
            Debug.Log($"플레이어가 가판대 {stallIndex + 1}에 접근했습니다. 무기를 구매하려면 Q 키를 누르세요.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            shopManager.SetPlayerInRange(stallIndex, false);
            Debug.Log($"플레이어가 가판대 {stallIndex + 1}에서 나갔습니다. 무기 구매가 불가능합니다.");
        }
    }
}