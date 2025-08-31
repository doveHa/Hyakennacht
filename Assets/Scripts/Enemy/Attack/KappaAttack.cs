using System;
using System.Collections;
using Manager;
using UnityEngine;

namespace Enemy.Attack
{
    public class KappaAttack : IAttack
    {
        public float ballSpeed;
        private GameObject _waterBall;
        public async void Start()
        {
            _waterBall = await AddressableManager.Manager.LoadAsset<GameObject>("Assets/Enemy/Resource/Kappa/Effect/WaterBall.prefab");

        }
        public override void Attack(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction = direction.normalized;
            GameObject ball = Instantiate(_waterBall, transform.position, transform.rotation);
            ball.GetComponent<Rigidbody2D>().AddForce(direction * ballSpeed);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}