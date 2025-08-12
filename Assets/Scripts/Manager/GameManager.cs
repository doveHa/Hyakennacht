using System.Collections;
using Character;
using UnityEngine;

namespace Manager
{
    public class GameManager : AbstractManager<GameManager>
    {
        private bool _isLeftSight = true;
        public GameObject Player { get; private set; }
        public Player PlayerScript { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        void Start()
        {
            PlayerScript = Player.GetComponent<Player>();
        }

        public void SetMoveVector(Vector2 movement)
        {
            Player.transform.GetComponent<Movement>().MoveVector = movement;
        }

        public void PlayerSight(bool isLeft)
        {
            if (isLeft)
            {
                Player.transform.rotation = Constant.Flip.NOTFLIPPED;
                _isLeftSight = true;
            }
            else
            {
                Player.transform.rotation = Constant.Flip.FLIPPED;
                _isLeftSight = false;
            }
        }

        public void Roll()
        {
            if (_isLeftSight)
            {
                StartCoroutine(Rolling());
            }
            else
            {
                StartCoroutine(Rolling());
            }
        }

        public void GameOver()
        {
            
        }

        private IEnumerator Rolling()
        {
            int currentFrame = 0;
            while (currentFrame < Constant.Roll.ROLL_FRAME)
            {
                currentFrame++;
                if (currentFrame > Constant.Roll.START_FRAME)
                {
                    if (_isLeftSight)
                    {
                        Player.transform.position += new Vector3((-1) * Constant.Roll.ROLL_DISTANCE, 0, 0);
                    }
                    else
                    {
                        Player.transform.position += new Vector3(Constant.Roll.ROLL_DISTANCE, 0, 0);
                    }
                }

                yield return null;
            }
        }
    }
}