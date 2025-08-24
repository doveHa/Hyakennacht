using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class FactionSelect : MonoBehaviour
    {
        public void SelectFaction(string faction)
        {
            SceneChangeManager.Manager.LoadLobbyScene(faction);
        }
    }
}