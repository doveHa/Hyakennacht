using UnityEngine;

public class StallTrigger : MonoBehaviour
{
    public int stallIndex;
    public ShopManager shopManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            /*            if (shopManager != null && stallIndex >= 0 && stallIndex < shopManager.stallTransforms.Length)
                        {
                            shopManager.SetPlayerInRange(stallIndex, true);
                            Debug.Log($"shopManager: {shopManager}, stallIndex: {stallIndex}, stallTransforms length: {shopManager.stallTransforms.Length}");
                            Debug.Log($"플레이어가 가판대 {stallIndex + 1}에 접근했습니다. 무기를 구매하려면 Q 키를 누르세요.");
                        }
                        else
                        {
                            Debug.LogError($"StallTrigger: 잘못된 stallIndex({stallIndex}) 또는 ShopManager 연결 오류");
                            Debug.LogError($"shopManager: {shopManager}, stallIndex: {stallIndex}, stallTransforms length: {(shopManager != null ? shopManager.stallTransforms.Length.ToString() : "null")}");
                        }*/
            var player = other.GetComponent<Player>();
            if (player != null && shopManager != null)
            {
                // 플레이어에 IInteractable 인터페이스와 인덱스를 전달합니다.
                player.SetCurrentInteractable(shopManager, stallIndex);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            /*            if (shopManager != null && stallIndex >= 0 && stallIndex < shopManager.stallTransforms.Length)
                        {
                            shopManager.SetPlayerInRange(stallIndex, false);
                            Debug.Log($"플레이어가 가판대 {stallIndex + 1}에서 나갔습니다. 무기 구매가 불가능합니다.");
                        }
                        else
                        {
                            Debug.LogError($"StallTrigger: 잘못된 stallIndex({stallIndex}) 또는 ShopManager 연결 오류");
                        }*/
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                // 플레이어의 상호작용 객체를 초기화합니다.
                player.ClearCurrentInteractable();
            }
        }
    }
}