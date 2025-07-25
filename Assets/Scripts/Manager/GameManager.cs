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
    }
}