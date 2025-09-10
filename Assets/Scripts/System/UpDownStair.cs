using Enemy;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace System
{
    public class UpDownStair : MonoBehaviour
    {
        public bool isUpStair;
        public bool IsEndStage { get; set; } = false;
        private bool _isPlayerInStair = false;
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.Equals("Player"))
            {
                _isPlayerInStair = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag.Equals("Player"))
            {
                _isPlayerInStair = false;
            }
        }

        void Update()
        {
            /*if (IsEndStage && Input.GetKeyDown(KeyCode.F))
            {
                GameManager.Manager.Player.GetComponent<PlayerCollision>().GuideKey.SetActive(false);
                MapManager.NextStage(isUpStair);;
            }*/
            if (_isPlayerInStair && Input.GetKeyDown(KeyCode.F))
            {
                // HR: 인자 있는 OnStageEnd 함수를 호출
                if (MapUIManager.Instance != null)
                {
                    MapUIManager.Instance.OnStageEnd(isUpStair);
                }
            }
        }
    }
}