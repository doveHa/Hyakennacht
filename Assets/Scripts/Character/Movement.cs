using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        private Vector3 _movement;

        void Start()
        {
            _movement = Vector3.zero;
        }
        
        void Update()
        {
            GameManager.Manager.Player.transform.position += _movement;
        }

        public void SetMovement(Vector2 movement)
        {
            _movement = movement;
        }
    }
}