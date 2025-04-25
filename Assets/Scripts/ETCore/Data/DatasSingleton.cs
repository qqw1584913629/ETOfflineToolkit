using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace MH
{
    /// <summary>
    /// 数据管理单例类
    /// 负责游戏数据的初始化、加载、保存等操作
    /// 实现了单例模式和自动唤醒接口
    /// </summary>
    public class DatasSingleton : LogicSingleton<DatasSingleton>, ISingletonAwake
    {
        /// <summary>
        /// JSON序列化设置
        /// 配置类型处理和循环引用处理
        /// </summary>
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        /// <summary>
        /// 需要进行数据变更监控的类型集合
        /// </summary>
        public HashSet<Type> EntityChangeTypeSet { get; } = new HashSet<Type>();

        /// <summary>
        /// 初始化方法
        /// 在系统启动时自动调用，完成数据系统的初始化
        /// </summary>
        public void Awake()
        {
            Init.Instance.Root = new Scene(IdGenerater.GenerateId(), SceneType.Main, "Main");
            Init.Instance.Root.AddComponent<TimerComponent>();
            
            EntityChangeTypeSet.Clear();
            foreach (Type type in CodeTypes.Instance.GetTypes().Values)
            {
                if (type != typeof(ISave) && typeof(ISave).IsAssignableFrom(type))
                    EntityChangeTypeSet.Add(type);
            }

            // 先创建指定的优先类型
            Type[] priorityTypes = new Type[]
            {
            };
            
            Type[] delayTypes = new Type[]
            {
            };

            // 处理优先类型
            foreach (var type in priorityTypes)
            {
                if (EntityChangeTypeSet.Contains(type))
                {
                    Debug.Log($"正在初始化优先数据：{type.Name}");
                    var createMethod = typeof(DatasSingleton).GetMethod("Create")?.MakeGenericMethod(type);
                    createMethod?.Invoke(this, null);
                }
            }

            // 处理普通类型（排除优先类型和延迟类型）
            foreach (var type in EntityChangeTypeSet)
            {
                if (priorityTypes.Contains(type) || delayTypes.Contains(type))
                    continue;
                Debug.Log($"正在初始化玩家数据：{type.Name}");
                var createMethod = typeof(DatasSingleton).GetMethod("Create")?.MakeGenericMethod(type);
                createMethod?.Invoke(this, null);
            }

            // 最后处理延迟类型
            foreach (var type in delayTypes)
            {
                if (EntityChangeTypeSet.Contains(type))
                {
                    Debug.Log($"正在初始化延迟数据：{type.Name}");
                    var createMethod = typeof(DatasSingleton).GetMethod("Create")?.MakeGenericMethod(type);
                    createMethod?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// 创建或加载指定类型的组件
        /// 如果数据不存在则创建新实例，否则加载已保存的数据
        /// </summary>
        /// <typeparam name="T">要创建或加载的组件类型</typeparam>
        /// <returns>创建或加载的组件实例</returns>
        public T Create<T>() where T : Entity, IAwake
        {
            var data = LoadDataByPlayerPrefs<T>();
            if (data == null)
            {
                var component = Init.Instance.Root.AddComponent<T>();
                SaveDataByPlayerPrefs(component);
                return component;
            }
            //反序列化
            Init.Instance.Root.AddComponent(data);
            Debug.Log($"{data.GetType().Name}：{JsonConvert.SerializeObject(data, settings)}");
            return data;
        }
        public void SaveAllDataByPlayerPrefs()
        {
            foreach (var type in EntityChangeTypeSet)
            {
                var component = Init.Instance.Root.GetComponent(type);
                SaveDataByPlayerPrefs(component);
            }
        }
        public void SaveDataByPlayerPrefs(Entity value)
        {
            if (value is not ISave)
                return;
            value.BeginInit();
            var json = JsonConvert.SerializeObject(value, settings);
            PlayerPrefs.SetString(value.GetType().Name, json);
            PlayerPrefs.Save();
        }
        private T LoadDataByPlayerPrefs<T>() where T : Entity
        {
            var json = PlayerPrefs.GetString(typeof(T).Name, defaultValue: default);
            if (json.Equals(default))
                return null;
            Debug.Log($"正在初始化玩家数据：{json}");
            T entity = JsonConvert.DeserializeObject<T>(json, settings);
            return entity;
        }
    }
}