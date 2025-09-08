using System;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.Attack
{
    public class KappaAttack : IAttack
    {
        public float ballSpeed;
        private Vector3 _direction;
        public void InstantiateWaterBall()
        {
            GameObject waterBall = transform.Find("WaterBall").gameObject;
            GameObject cloneBall = Instantiate(waterBall, waterBall.transform.position, waterBall.transform.rotation);
            cloneBall.SetActive(true);
            cloneBall.GetComponent<Rigidbody2D>().AddForce(_direction * ballSpeed);
        }
    
        public override void Attack(Vector3 targetPosition)
        {
            _direction = (targetPosition - transform.position).normalized;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}