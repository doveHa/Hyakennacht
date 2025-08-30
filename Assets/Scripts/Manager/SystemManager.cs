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
        private TextMeshProUGUI _coinCount;
        //public TextMeshProUGUI _coinCount;
        //HR: shopManager에서 결제에 쓰려고 퍼블릭으로 바꿨는데 이거 때문에 Manager에서 씬 이동 시 에러가 났던걸까요?

        protected override void Awake()
        {
            base.Awake();
            Instance = this; //HR: 싱글톤
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