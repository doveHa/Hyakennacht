/*
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Manager
{
    public class AddressableManager : AbstractManager<AddressableManager>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public async Task<T> LoadAsset<T>(string key)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            T t = handle.Result;
            Addressables.Release(handle);
            return t;
        }
    }
}*/