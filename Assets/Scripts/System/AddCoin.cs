using Manager;
using UnityEngine;

public class AddCoin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameManager.Manager.PlayerScript.PlayerGetCoin();
            Destroy(gameObject);

        }
    }
}
