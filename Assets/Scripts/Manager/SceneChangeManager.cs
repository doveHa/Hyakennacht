using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneChangeManager : AbstractManager<SceneChangeManager>
    {
        protected override void Awake()
        {
            Debug.Log("start");
            base.Awake();
        }

        public void LoadLobbyScene(string faction)
        {
            switch (faction)
            {
                case "Witch":
                    SceneManager.LoadScene(Constant.Scene.WITCHLOBBY);
                    break;
                case "Yokai":
                    SceneManager.LoadScene(Constant.Scene.YOKAILOBBY);
                    break;
            }
        }
    }
}