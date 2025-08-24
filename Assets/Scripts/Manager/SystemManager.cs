using System;
using TMPro;

namespace Manager
{
    public class SystemManager : AbstractManager<SystemManager>
    {
        public HpControl hpControl;
        public TextMeshProUGUI coinCount;
        
        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
            coinCount.text = GameManager.Manager.PlayerScript.Coins.ToString();
        }
    }
}