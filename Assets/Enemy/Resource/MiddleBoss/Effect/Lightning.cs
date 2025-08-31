using System.Collections;
using Manager;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public float _remainingTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(transform.parent.gameObject);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(_remainingTime);
        
        Destroy(transform.parent.gameObject);
    }
}
