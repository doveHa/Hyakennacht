using UnityEngine;

public class StallTrigger : MonoBehaviour
{
    public int stallIndex;
    public ShopManager shopManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (shopManager != null && stallIndex >= 0 && stallIndex < shopManager.stallTransforms.Length)
            {
                shopManager.SetPlayerInRange(stallIndex, true);
                Debug.Log($"shopManager: {shopManager}, stallIndex: {stallIndex}, stallTransforms length: {shopManager.stallTransforms.Length}");
                Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1}�� �����߽��ϴ�. ���⸦ �����Ϸ��� Q Ű�� ��������.");
            }
            else
            {
                Debug.LogError($"StallTrigger: �߸��� stallIndex({stallIndex}) �Ǵ� ShopManager ���� ����");
                Debug.LogError($"shopManager: {shopManager}, stallIndex: {stallIndex}, stallTransforms length: {(shopManager != null ? shopManager.stallTransforms.Length.ToString() : "null")}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (shopManager != null && stallIndex >= 0 && stallIndex < shopManager.stallTransforms.Length)
            {
                shopManager.SetPlayerInRange(stallIndex, false);
                Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1}���� �������ϴ�. ���� ���Ű� �Ұ����մϴ�.");
            }
            else
            {
                Debug.LogError($"StallTrigger: �߸��� stallIndex({stallIndex}) �Ǵ� ShopManager ���� ����");
            }
        }
    }
}