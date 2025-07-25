using Manager;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        private Vector3 _movement;
        private bool _isSightLeft;

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

        public void StartRoll()
        {
            Vector2 playerPosition = GameManager.Manager.Player.transform.position;
            CoffTest test = GameObject.Find("Coff").GetComponent<CoffTest>();
            if (_isSightLeft)
            {
                playerPosition.x += (-1) * Constant.SPEED_DISTANCE.ROLL_DISTANCE;
                //playerPosition.x += (-1) * test.RollDistance;
            }
            else
            {
                playerPosition.x += Constant.SPEED_DISTANCE.ROLL_DISTANCE;
                //playerPosition.x += test.RollDistance;
            }

            GameManager.Manager.Player.transform.position = playerPosition;
        }

        public void SetSightLeft(bool isLeft)
        {
            _isSightLeft = isLeft;
        }
    }
}