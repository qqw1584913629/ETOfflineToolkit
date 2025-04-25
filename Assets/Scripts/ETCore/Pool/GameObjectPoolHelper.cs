using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH
{
public static class GameObjectPoolHelper
    {
        private static Dictionary<string, GameObjectPool> poolDict = new Dictionary<string, GameObjectPool>();

        public static bool IsContainPool(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="size">初始大小</param>
        /// <param name="type">增长类型</param> 
        public static void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = GetGameObjectByResType(poolName);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }


                    poolDict[poolName] = new GameObjectPool(poolName, pb, GameObject.Find("Global/PoolRoot"), size, type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 异步获取游戏对象
        /// </summary>
        /// <param name="gameObjectPath">游戏对象路径</param>
        /// <returns>游戏对象</returns>
        public static async ETTask<GameObject> GetGameObjectAsync(string gameObjectPath)
        {
            GameObject pb = null;
            pb = await ResourceSingleton.Instance.LoadAssetAsync<GameObject>(gameObjectPath);
            return pb;
        }
        /// <summary>
        /// 异步初始化对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="assetPath">资源路径</param>
        /// <param name="size">初始大小</param>
        /// <param name="type">增长类型</param> 
        public static async ETTask InitPoolWithPathAsync(string poolName, string assetPath, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = await GetGameObjectAsync(assetPath);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }

                    poolDict[poolName] = new GameObjectPool(poolName, pb, GameObject.Find("Global/PoolRoot"), size, type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }



        /// <summary>
        /// 从对象池获取一个可用对象
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="autoActive">是否自动激活</param>
        /// <param name="autoCreate">自动创建数量</param>
        /// <returns>游戏对象</returns>
        public static GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                GameObjectPool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("[ResourceManager]:Invalid pool name specified: " + poolName);
                // Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }


        /// <summary>
        /// 异步从对象池获取一个可用对象
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="assetPath">资源路径</param>
        /// <param name="autoActive">是否自动激活</param>
        /// <param name="autoCreate">自动创建数量</param>
        /// <returns>游戏对象</returns>
        public static async ETTask<GameObject> GetObjectFromPoolAsync(string poolName, string assetPath, bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                await InitPoolWithPathAsync(poolName, assetPath, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                GameObjectPool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }


        /// <summary>
        /// 返回对象到对象池
        /// </summary>
        /// <param name="go">游戏对象</param>
        public static void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                GameObjectPool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// 返回对象到对象池
        /// </summary>
        /// <param name="t">Transform</param>
        public static void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

        /// <summary>
        /// 通过资源类型获取游戏对象
        /// </summary>
        /// <param name="poolName">资源类型</param>
        /// <returns>游戏对象</returns>
        public static GameObject GetGameObjectByResType(string poolName)
        {
            GameObject pb = ResourceSingleton.Instance.LoadAssetSync<GameObject>(poolName);
            return pb;
        }
    }
}