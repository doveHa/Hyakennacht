using Enemy;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace System
{
    public class RoomControl : MonoBehaviour
    {
        public bool isFirstRoom;
        private EnemySpawner _spawner;

        void Start()
        {
            _spawner = GetComponent<EnemySpawner>();
            if (isFirstRoom)
            {
                Destroy(_spawner);
            }
        }
        
        public async void StartStage()
        {
            TileBase tileBase = await AddressableManager.Manager.LoadAsset<TileBase>("Assets/Tile/256Wall.asset");
            Tilemap tilemap = transform.AddComponent<Tilemap>();
            MapManager.GenerateWalls(GetComponent<Tilemap>(), tilemap, tileBase);
            _spawner.IsSpawn = false;
        }

        public void EndStage()
        {
            Destroy(_spawner);
        }
    }
}