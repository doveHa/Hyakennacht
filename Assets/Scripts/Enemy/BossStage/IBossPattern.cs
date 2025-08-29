using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.BossStage
{
    public abstract class ABossPattern : MonoBehaviour
    {
        protected List<Action> _actions;
        protected Transform _target;

        void Awake()
        {
            _actions = new List<Action>();
        }
        protected virtual void Start()
        {
            _target = GameManager.Manager.PlayerScript.Target;
            SetAction();
        }
        
        public Action RandomPattern()
        {
            return _actions[Random.Range(0, _actions.Count)];
        }

        protected abstract void SetAction();
    }
}