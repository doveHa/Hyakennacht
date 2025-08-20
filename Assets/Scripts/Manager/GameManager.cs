using System.Collections;
using Character;
using UnityEngine;

namespace Manager
{
    public class GameManager : AbstractManager<GameManager>
    {
        public GameObject Player { get; private set; }
        public Player PlayerScript { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerScript = Player.GetComponent<Player>();
        }
        
        public void SetMoveVector(Vector2 movement)
        {
            Player.transform.GetComponentInChildren<Movement>().MoveVector = movement;
        }

        public void PlayerSight(bool isLeft)
        {
            Player.GetComponentInChildren<SpriteRenderer>().flipX = !isLeft;
            Player.GetComponentInChildren<Dash>().IsLeftSight = isLeft;
        }
        
        public void GameOver()
        {
        }
    }
}