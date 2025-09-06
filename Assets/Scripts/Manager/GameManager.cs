using Character;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : AbstractManager<GameManager>
    {
        public GameObject Player { get; private set; }
        public Player PlayerScript { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            //HR: 씬 전환 후 Player 재설정
            RefreshPlayerReference();
            SceneManager.sceneLoaded += OnSceneLoaded;

            //Player = GameObject.FindGameObjectWithTag("Player");
            //PlayerScript = Player.GetComponent<Player>();
        }

        //HR: 씬 전환
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshPlayerReference();
        }

        public void RefreshPlayerReference()
        {
            var scenePlayer = GameObject.FindGameObjectWithTag("Player");
            if (scenePlayer != null)
            {
                Player = scenePlayer;
                PlayerScript = Player.GetComponent<Player>();
            }
        }


        public void SetMoveVector(Vector2 movement)
        {
            if (Player != null)
                Player.GetComponentInChildren<Character.Movement>().MoveVector = movement;
        }

        public void PlayerSight(bool isLeft)
        {
            if (Player != null)
            {
                Player.GetComponentInChildren<SpriteRenderer>().flipX = !isLeft;
                Player.GetComponentInChildren<Dash>().IsLeftSight = isLeft;
            }
        }
        
        public void GameOver()
        {
            Time.timeScale = 0f;
            MapUIManager.Instance.PlayerDied = true;  //HR false -> true
            Debug.Log(MapUIManager.Instance.PlayerDied);
            MapUIManager.Instance.OnStageEnd();
        }

        //HR
        public void SetPlayer(GameObject player)
        {
            Player = player;
            PlayerScript = player.GetComponent<Player>();
        }

    }
}