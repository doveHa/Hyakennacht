using Manager;
using UnityEngine;

namespace System
{
    public class HpControl : MonoBehaviour
    {
        public GameObject[] hpArray;
        private int _currentHp;

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
            hpArray[_currentHp--].SetActive(false);

            if (_currentHp == 0)
            {
                GameManager.Manager.GameOver();
            }
        }
    }
}