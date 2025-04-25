using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH
{
    public class EntitySystemSingleton : LogicSingleton<EntitySystemSingleton>, ISingletonAwake
    {
        /// <summary>
        /// 存储所有系统对象的集合
        /// 键为系统类型，值为系统对象的列表
        /// </summary>
        private Dictionary<Type, Dictionary<Type, List<SystemObject>>> _allSystemObjects = new Dictionary<Type, Dictionary<Type, List<SystemObject>>>();

        /// <summary>
        /// 初始化方法
        /// 在系统启动时自动调用，完成系统对象的初始化
        /// </summary>
        public void Awake()
        {
            foreach (Type type in CodeTypes.Instance.GetTypes(typeof(EntitySystemAttribute)))
            {
                SystemObject obj = (SystemObject)Activator.CreateInstance(type);
                if (obj is ISystemType iSystemType)
                {
                    if (!_allSystemObjects.ContainsKey(iSystemType.Type()))
                    {
                        _allSystemObjects.Add(iSystemType.Type(), new Dictionary<Type, List<SystemObject>>());
                    }
                    var allSystemObject = _allSystemObjects[iSystemType.Type()];
                    if (!allSystemObject.ContainsKey(iSystemType.SystemType()))
                    {
                        allSystemObject.Add(iSystemType.SystemType(), new List<SystemObject>());
                    }
                    allSystemObject[iSystemType.SystemType()].Add(obj);
                }
            }
            for (int i = 0; i < this.queues.Length; i++)
            {
                this.queues[i] = new Queue<Entity>();
            }
        }

        /// <summary>
        /// 获取指定类型和系统类型的系统对象列表
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <param name="systemType">系统对象类型</param>
        public List<SystemObject> GetSystems(Type type, Type systemType)
        {
            Dictionary<Type, List<SystemObject>> res = null;
            List<SystemObject> res2 = null;
            if (!this._allSystemObjects.TryGetValue(type, out res))
                return null;
            if (!res.TryGetValue(systemType, out res2))
                return null;
            return res2;
        }
        /// <summary>
        /// 获取指定类型的所有系统对象列表
        /// </summary>
        /// <param name="type">系统类型</param>
        public Dictionary<Type, List<SystemObject>> GetSystems(Type type)
        {
            Dictionary<Type, List<SystemObject>> res = null;
            if (!this._allSystemObjects.TryGetValue(type, out res))
                return null;
            return res;
        }

        private readonly Queue<Entity>[] queues = new Queue<Entity>[InstanceQueueIndex.Max];
        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public virtual void RegisterSystem(Entity component)
        {
            var entitySystems = GetSystems(component.GetType());
            if (entitySystems == null)
                return;
            foreach (var lists in entitySystems.Values)
            {
                foreach (var systemObject in lists)
                {
                    if (systemObject is IUpdateSystem)
                        queues[InstanceQueueIndex.Update].Enqueue(component);
                    if (systemObject is ILateUpdateSystem)
                        queues[InstanceQueueIndex.LateUpdate].Enqueue(component);
                }
            }
        }
        /// <summary>
        /// 唤醒系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public void Awake(Entity component)
        {
            List<SystemObject> iAwakeSystems = GetSystems(component.GetType(), typeof(IAwakeSystem));
            if (iAwakeSystems == null)
                return;
            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 唤醒系统
        /// </summary>
        /// <param name="component">组件实例</param>
        /// <param name="p1">参数1</param>
        public void Awake<P1>(Entity component, P1 p1)
        {
            if (component is not IAwake<P1>)
            {
                return;
            }
            List<SystemObject> iAwakeSystems = GetSystems(component.GetType(), typeof(IAwakeSystem<P1>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 唤醒系统
        /// </summary>
        /// <param name="component">组件实例</param>
        /// <param name="p1">参数1</param>
        /// <param name="p2">参数2</param>
        public void Awake<P1, P2>(Entity component, P1 p1, P2 p2)
        {
            if (component is not IAwake<P1, P2>)
            {
                return;
            }
            List<SystemObject> iAwakeSystems = GetSystems(component.GetType(), typeof(IAwakeSystem<P1, P2>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1, P2> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1, p2);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 唤醒系统
        /// </summary>
        /// <param name="component">组件实例</param>
        /// <param name="p1">参数1</param>
        /// <param name="p2">参数2</param>
        /// <param name="p3">参数3</param>
        public void Awake<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
        {
            if (component is not IAwake<P1, P2, P3>)
            {
                return;
            }
            List<SystemObject> iAwakeSystems = GetSystems(component.GetType(), typeof(IAwakeSystem<P1, P2, P3>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1, P2, P3> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1, p2, p3);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 销毁系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public void Destroy(Entity component)
        {
            if (component is not IDestroy)
            {
                return;
            }
            List<SystemObject> iDestroySystems = GetSystems(component.GetType(), typeof(IDestroySystem));
            if (iDestroySystems == null)
            {
                return;
            }

            foreach (IDestroySystem iDestroySystem in iDestroySystems)
            {
                if (iDestroySystem == null)
                {
                    continue;
                }

                try
                {
                    iDestroySystem.Run(component);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public void Update(Entity component)
        {
            if (component is not IUpdate)
                return;
            List<SystemObject> iUpdateSystems = GetSystems(component.GetType(), typeof(IUpdateSystem));
            if (iUpdateSystems == null)
                return;
            foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
            {
                try
                {
                    iUpdateSystem.Run(component);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        public void Update()
        {
            Queue<Entity> queue = this.queues[InstanceQueueIndex.Update];
            int count = queue.Count;
            while (count-- > 0)
            {
                Entity component = queue.Dequeue();
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                if (component is not IUpdate)
                {
                    continue;
                }

                try
                {
                    List<SystemObject> iUpdateSystems = GetSystems(component.GetType(), typeof(IUpdateSystem));
                    if (iUpdateSystems == null)
                    {
                        continue;
                    }

                    queue.Enqueue(component);

                    foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
                    {
                        try
                        {
                            iUpdateSystem.Run(component);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"entity system update fail: {component.GetType().FullName}", e);
                }
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        public void LateUpdate()
        {
            Queue<Entity> queue = this.queues[InstanceQueueIndex.LateUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                Entity component = queue.Dequeue();
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                if (component is not ILateUpdate)
                {
                    continue;
                }

                List<SystemObject> iLateUpdateSystems = GetSystems(component.GetType(), typeof(ILateUpdateSystem));
                if (iLateUpdateSystems == null)
                {
                    continue;
                }

                queue.Enqueue(component);

                foreach (ILateUpdateSystem iLateUpdateSystem in iLateUpdateSystems)
                {
                    try
                    {
                        iLateUpdateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public void LateUpdate(Entity component)
        {
            if (component is not ILateUpdate)
                return;
            List<SystemObject> iLateUpdateSystems = GetSystems(component.GetType(), typeof(ILateUpdateSystem));
            if (iLateUpdateSystems == null)
                return;
            foreach (ILateUpdateSystem iLateUpdateSystem in iLateUpdateSystems)
            {
                try
                {
                    iLateUpdateSystem.Run(component);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 更新系统
        /// </summary>
        /// <param name="component">组件实例</param>
        public void Deserialize(Entity component)
        {
            if (component is not IDeserialize)
            {
                return;
            }

            List<SystemObject> iDeserializeSystems = GetSystems(component.GetType(), typeof(IDeserializeSystem));
            if (iDeserializeSystems == null)
            {
                return;
            }

            foreach (IDeserializeSystem deserializeSystem in iDeserializeSystems)
            {
                if (deserializeSystem == null)
                {
                    continue;
                }

                try
                {
                    deserializeSystem.Run(component);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}