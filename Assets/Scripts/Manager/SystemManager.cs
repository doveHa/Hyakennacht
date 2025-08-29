using System;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class SystemManager : AbstractManager<SystemManager>
    {
        //HR: 싱글톤
        public static SystemManager Instance { get; private set; }

        public HpControl HpControl { get; private set; }
        //private TextMeshProUGUI _coinCount;
        public TextMeshProUGUI _coinCount;

        protected override void Awake()
        {
            base.Awake();
            Instance = this; //HR: 싱글톤
            HpControl = FindFirstObjectByType<HpControl>();
            _coinCount = GameObject.Find("GameSystem").GetComponentInChildren<TextMeshProUGUI>();
        }

        void Update()
        {
            _coinCount.text = GameManager.Manager.PlayerScript.Coins.ToString();
        }
    }
}