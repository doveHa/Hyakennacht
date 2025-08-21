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
            Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1}�� �����߽��ϴ�. ���⸦ �����Ϸ��� Q Ű�� ��������.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            shopManager.SetPlayerInRange(stallIndex, false);
            Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1}���� �������ϴ�. ���� ���Ű� �Ұ����մϴ�.");
        }
    }
}