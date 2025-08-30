using Manager;
using UnityEngine;

namespace System
{
    public class HpControl : MonoBehaviour
    {
        public GameObject[] hpArray;
        private int _currentHp;
        private bool _isInvincible;

        //hr
        public int MaxHp => hpArray.Length;
        public int CurrentHp => _currentHp;
        public bool IsFullHp => _currentHp >= MaxHp - 1;

        void Awake()
        {
            _currentHp = 4;
        }

        public void PlusHp()
        {
            if (_currentHp + 1 < hpArray.Length)
            {
                hpArray[++_currentHp].SetActive(true);
            }
        }

        public void MinusHp()
        {
            if (!_isInvincible)
            {
                hpArray[_currentHp--].SetActive(false);

                if (_currentHp == 0)
                {
                    GameManager.Manager.GameOver();
                }
            }
        }

        public void SetInvincible(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }
    }
}