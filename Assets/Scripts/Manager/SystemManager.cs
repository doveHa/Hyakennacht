using System;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class SystemManager : AbstractManager<SystemManager>
    {
        public HpControl HpControl { get; private set; }
        private TextMeshProUGUI _coinCount;

        protected override void Awake()
        {
            base.Awake();
            HpControl = FindFirstObjectByType<HpControl>();
            _coinCount = GameObject.Find("GameSystem").GetComponentInChildren<TextMeshProUGUI>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                HpControl.PlusHp();
            }
            _coinCount.text = GameManager.Manager.PlayerScript.Coins.ToString();
        }
    }
}