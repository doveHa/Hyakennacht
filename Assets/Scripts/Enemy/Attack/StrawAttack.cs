using System;
using System.Collections;
using UnityEngine;

namespace Enemy.Attack
{
    public class StrawAttack : IAttack
    {
        public void InstantiateStrawDoll()
        {
            GameObject doll = transform.GetChild(2).gameObject;
            GameObject cloneDoll = Instantiate(doll, doll.transform.position, doll.transform.rotation);
            cloneDoll.SetActive(true);
        }
        public override void Attack(Vector3 targetPosition)
        {
            transform.GetChild(2).GetComponent<SmallStraw>().SetTarget(targetPosition);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}