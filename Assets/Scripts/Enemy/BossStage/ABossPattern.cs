using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.BossStage
{
    public abstract class ABossPattern : MonoBehaviour
    {
        public List<Action> Actions { get; private set; }
        protected Transform _target;

        void Awake()
        {
            Actions = new List<Action>();
        }
        protected virtual void Start()
        {
            _target = GameManager.Manager.PlayerScript.Target;
            SetAction();
        }
        
        public int RandomPattern()
        {
            return Random.Range(0, Actions.Count);
        }

        protected abstract void SetAction();
    }
}