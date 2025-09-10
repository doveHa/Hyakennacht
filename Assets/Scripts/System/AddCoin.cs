using Manager;
using UnityEngine;

public class AddCoin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameManager.Manager.PlayerScript.PlayerGetCoin();

            //HR
            if (MapUIManager.Instance != null)
            {
                MapUIManager.Instance.AddStageCoins(1); // 코인을 1씩 추가
            }

            Destroy(gameObject);
        }
    }
}
