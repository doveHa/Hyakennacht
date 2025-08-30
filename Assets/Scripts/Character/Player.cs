using Manager;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    public WeaponHandler WeaponHandler { get; private set; }
    public int Coins { get; private set; }
    public Transform Target { get; private set; }

    void Awake()
    {  
        DontDestroyOnLoad(this);
        Instantiate(playerPrefab, transform).transform.parent = transform;
        WeaponHandler = GetComponent<WeaponHandler>();
        Target = transform.GetChild(0).GetChild(1);
        Coins = 0;
    }
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void Hit()
    {
        SystemManager.Manager.HpControl.MinusHp();
    }
    public void PlayerGetCoin()
    {
        Coins++;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag.Equals("Projectile"))
        {
            SystemManager.Manager.HpControl.MinusHp();
            Destroy(other.gameObject);
        }
    }
}