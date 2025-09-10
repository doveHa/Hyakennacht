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
            if (IsEndStage && Input.GetKeyDown(KeyCode.F))
            {
                GameManager.Manager.Player.GetComponent<PlayerCollision>().GuideKey.SetActive(false);
                MapManager.NextStage(isUpStair);;
            }
            
        }
    }
}