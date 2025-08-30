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
            Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1} ���� ������ ���Խ��ϴ�.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            selector.SetPlayerInRange(stallIndex, false);
            Debug.Log($"�÷��̾ ���Ǵ� {stallIndex + 1} �������� �������ϴ�.");
        }
    }
}
