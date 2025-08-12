using System;
using Character;
using Enemy;
using Manager;
using UnityEngine;
using Random = System.Random;

public class RandomMove : MonoBehaviour
{
    public float moveSpeed;
    private int _moveX, _moveY;
    
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        Random rnd = new Random();

        _moveX = rnd.Next(-1, 1);
        _moveY = rnd.Next(-1, 1);
        
        Vector3 moveVector = new Vector2(_moveX, _moveY);
        transform.position -= moveVector * moveSpeed;
        
        Initialize();
    }

    private void Initialize()
    {
        _moveX = 0;
        _moveY = 0;        
    }
}
