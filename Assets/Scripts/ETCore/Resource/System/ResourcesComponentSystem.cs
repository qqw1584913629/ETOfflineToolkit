using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;

namespace MH
{
    [EntitySystem]
    public class ResourcesComponentAwakeSystem : AwakeSystem<ResourcesComponent>
    {
        protected override void Awake(ResourcesComponent self)
        {
            self.package = YooAssets.GetPackage("DefaultPackage");
        }
    }
    [EntitySystem]
    public class ResourcesComponentAwakeASystem : AwakeSystem<ResourcesComponent, string>
    {
        protected override void Awake(ResourcesComponent self, string packageName)
        {
            self.package = YooAssets.GetPackage(packageName);
        }
    }
    [EntitySystem]
    public class ResourcesComponentDestroySystem : DestroySystem<ResourcesComponent>
    {
        protected override void Destroy(ResourcesComponent self)
        {
            foreach (var kv in self.handlers)
            {
                switch (kv.Value)
                {
                    case AssetOperationHandle handle:
                        handle.Release();
                        break;
                    case AllAssetsOperationHandle handle:
                        handle.Release();
                        break;
                    case SubAssetsOperationHandle handle:
                        handle.Release();
                        break;
                    case RawFileOperationHandle handle:
                        handle.Release();
                        break;
                    case SceneOperationHandle handle:
                        if (!handle.IsMainScene())
                            handle.UnloadAsync();
                        break;
                }
            }
        }
    }

    public static class ResourcesComponentSystem
    {
        public static void ReleaseHandler(this ResourcesComponent self, OperationHandleBase handleBase)
        {
            switch (handleBase)
            {
                case AssetOperationHandle handle:
                    handle.Release();
                    break;
                case AllAssetsOperationHandle handle:
                    handle.Release();
                    break;
                case SubAssetsOperationHandle handle:
                    handle.Release();
                    break;
                case RawFileOperationHandle handle:
                    handle.Release();
                    break;
                case SceneOperationHandle handle:
                    if (!handle.IsMainScene())
                        handle.UnloadAsync();
                    break;
            }
        }
        public static void UnLoadAssetSync(this ResourcesComponent self, string location)
                {
                    OperationHandleBase handler;
                    if (self.handlers.TryGetValue(location, out handler))
                    {
                        self.ReleaseHandler(handler);
                        self.handlers.Remove(location);
                    }
                }
        
        
                public static T LoadAssetSync<T>(this ResourcesComponent self, string location) where T : UnityEngine.Object
                {
                    OperationHandleBase handler;
                    if (!self.handlers.TryGetValue(location, out handler))
                    {
                        handler = self.package.LoadAssetSync<T>(location);
        
                        self.handlers.Add(location, handler);
                    }
                    return (T)((AssetOperationHandle)handler).AssetObject;
                }
        
                public static async ETTask<T> LoadAssetAsync<T>(this ResourcesComponent self, string location) where T : UnityEngine.Object
                {
                    var coroutineLockComponent = self.Root.GetComponent<CoroutineLockComponent>();
                    using CoroutineLock coroutineLock = await coroutineLockComponent.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());
        
                    OperationHandleBase handler;
                    if (!self.handlers.TryGetValue(location, out handler))
                    {
                        handler = self.package.LoadAssetAsync<T>(location);
        
                        await handler.Task;
        
                        self.handlers.Add(location, handler);
                    }
        
                    return (T)((AssetOperationHandle)handler).AssetObject;
                }
        
                public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesComponent self, string location) where T : UnityEngine.Object
                {
                    using CoroutineLock coroutineLock = await self.Root.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());
        
                    OperationHandleBase handler;
                    if (!self.handlers.TryGetValue(location, out handler))
                    {
                        handler = self.package.LoadAllAssetsAsync<T>(location);
                        await handler.Task;
                        self.handlers.Add(location, handler);
                    }
        
                    Dictionary<string, T> dictionary = new Dictionary<string, T>();
                    foreach (UnityEngine.Object assetObj in ((AllAssetsOperationHandle)handler).AllAssetObjects)
                    {
                        T t = assetObj as T;
                        dictionary.Add(t.name, t);
                    }
        
                    return dictionary;
                }
        
                public static async ETTask LoadSceneAsync(this ResourcesComponent self, string location, LoadSceneMode loadSceneMode)
                {
                    using CoroutineLock coroutineLock = await self.Root.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());
        
                    OperationHandleBase handler;
                    if (self.handlers.TryGetValue(location, out handler))
                    {
                        return;
                    }
        
                    handler = self.package.LoadSceneAsync(location);
        
                    await handler.Task;
                    self.handlers.Add(location, handler);
                }
    }
}