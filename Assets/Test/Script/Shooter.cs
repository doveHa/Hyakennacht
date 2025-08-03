using System.Collections;
using Manager;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject ball;

    void Start()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            GenerateBall();
            yield return new WaitForSeconds(2f);
        }
    }

    private void GenerateBall()
    {
        Rigidbody2D rb = Instantiate(ball, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 direction = (GameManager.Manager.Player.transform.position - transform.position).normalized;
        rb.AddForce(direction * 3, ForceMode2D.Impulse);
    }
}