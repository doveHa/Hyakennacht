using Manager;
using UnityEngine;

public class WispBoard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag.Equals("Enemy"))
        {
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
