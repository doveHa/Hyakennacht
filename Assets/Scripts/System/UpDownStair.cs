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

        void OnTriggerStay2D(Collider2D other)
        {
            if (!IsEndStage)
            {
                return;
            }
            
            if (other.tag.Equals("Player") && Input.GetKeyDown(KeyCode.F))
            {
                GameManager.Manager.Player.GetComponent<PlayerCollision>().GuideKey.SetActive(false);
                MapManager.Instance.NextStage(isUpStair);
            }
        }
    }
}