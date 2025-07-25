using Character;
using UnityEngine;

namespace Manager
{
    public class GameManager : AbstractManager<GameManager>
    {
        public GameObject Player { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        public void MovePlayer(Vector2 movement)
        {
            Player.transform.GetComponent<Movement>().SetMovement(movement);
        }

        public void PlayerSight(bool isLeft)
        {
            if (isLeft)
            {
                Player.transform.rotation = Constant.FLIP.NOTFLIPPED;
            }
            else
            {
                Manager.Player.transform.rotation = Constant.FLIP.FLIPPED;
            }

            Player.transform.GetComponent<Movement>().SetSightLeft(isLeft);
        }

        public void Roll()
        {
            Player.transform.GetComponent<Movement>().StartRoll();
        }
    }
}