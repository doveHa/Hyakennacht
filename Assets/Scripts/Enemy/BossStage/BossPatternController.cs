using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Enemy.BossStage
{
    public class BossPatternController : MonoBehaviour
    {
        private ABossPattern _bossPattern;
        private EnemyController _controller;
        
        private float _patternTimer = 0f;
        public float patternDuration = 2f;
        
        void Start()
        {
            _controller = GetComponent<EnemyController>();
            _bossPattern = GetComponentInParent<ABossPattern>();
        }
        
        void Update()
        {
            _patternTimer += Time.deltaTime;

            if (_patternTimer >= patternDuration)
            {
                _patternTimer = 0f;

                Action action = _bossPattern.RandomPattern();
                
                _controller.ChangeState(new ActivePatternState(_controller, action));
            }
        }
        
    }
}