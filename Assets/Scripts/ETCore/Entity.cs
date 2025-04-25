using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace MH
{
    public abstract class Entity : IDisposable
    {
        private Dictionary<long, Entity> _children;
        [JsonIgnore]
        public Dictionary<long, Entity> Childrens => _children ??= new Dictionary<long, Entity>();
        public List<Entity> _childrenDB;
        public List<Entity> _componentDB;
        private Dictionary<long, Entity> _component;
        [JsonIgnore]
        public Dictionary<long, Entity> Components => _component ??= new Dictionary<long, Entity>();
        public long Id;
        [JsonIgnore]
        public Entity Parent
        {
            get => _parent;
            protected set
            {
                if (this._parent != null) // 之前有parent
                {
                    // parent相同，不设置
                    if (this._parent == value)
                        return;
                    this._parent.RemoveFromChildren(this);
                }

                _parent = value;
                this._isComponent = false;
                if (_parent != null)
                    this._parent.AddToChildren(this);

                if (this is Scene)
                {
                    this.Scene = this as Scene;
                }
                else
                {
                    this.Scene = value.Scene;
                }
            }
        }
        private Entity _parent;
        [JsonIgnore]
        public Scene Root => Init.Instance.Root;
        protected Scene scene;
        [JsonIgnore]
        public Scene Scene
        {
            get
            {
                return this.scene;
            }
            protected set
            {
                if (value == null)
                    throw new Exception($"domain cant set null: {this.GetType().FullName}");
                if (this.scene == value)
                    return;
                Scene preScene = this.scene;
                this.scene = value;

                if (preScene == null)
                {
                    this._isRegister = true;
                    this.RegisterSystem();
                    // 反序列化出来的需要设置父子关系
                    if (this._componentDB != null)
                    {
                        foreach (Entity component in this._componentDB)
                        {
                            component._isComponent = true;
                            this.Components.TryAdd(this.GetLongHashCode(component.GetType()), component);
                            component._parent = this;
                        }
                    }
                    if (this._childrenDB != null)
                    {
                        foreach (Entity child in this._childrenDB)
                        {
                            child._isComponent = false;
                            this.Childrens.TryAdd(child.Id, child);
                            child._parent = this;
                        }
                    }
                }

                // 递归设置孩子的Domain
                if (this._children != null)
                {
                    foreach (Entity entity in this._children.Values)
                    {
                        entity.Scene = this.scene;
                    }
                }

                if (this._component != null)
                {
                    foreach (Entity component in this._component.Values)
                    {
                        component.Scene = this.scene;
                    }
                }

                if (!this._isCreate)
                {
                    this._isCreate = true;
                    EntitySystemSingleton.Instance.Deserialize(this);
                }
            }
        }
        private bool _isCreate = false;
        public bool IsCreate => this._isCreate;
        private bool _isDisposed = false;
        public bool IsDisposed => this._isDisposed;
        private bool _isComponent = false;
        public bool IsComponent => this._isComponent;
        private bool _isRegister = false;
        public bool IsRegister => this._isRegister;
        protected virtual void RegisterSystem()
        {
            EntitySystemSingleton.Instance.RegisterSystem(this);
        }
        
        public static long GenerateID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
        /// <summary>
        /// 创建指定类型的实体实例
        /// </summary>
        /// <typeparam name="T">要创建的实体类型</typeparam>
        /// <returns>创建的实体实例</returns>
        private static Entity Create(Type type)
        {
            Entity component;
            component = Activator.CreateInstance(type) as Entity;
            component._isCreate = true;
            component.Id = 0;
            return component;
        }
        /// <summary>
        /// 创建指定类型的实体实例
        /// </summary>
        /// <typeparam name="T">要创建的实体类型</typeparam>
        /// <returns>创建的实体实例</returns>
        private static T Create<T>() where T : Entity
        {
            T obj = Activator.CreateInstance(typeof(T)) as T;
            return obj;
        }

        /// <summary>
        /// 添加组件到实体的组件字典中
        /// </summary>
        /// <param name="component">要添加的组件</param>
        void AddComponentChild(Entity component)
        {
            if (_component == null)
            {
                _component = new Dictionary<long, Entity>();
            }
            _component.Add(GetLongHashCode(component.GetType()), component);
        }
        
        /// <summary>
        /// 添加子实体，并设置父子关系
        /// </summary>
        /// <param name="entity">要添加的子实体</param>
        /// <returns>添加的子实体</returns>
        /// <exception cref="Exception">当父实体为空且当前实体不是Scene类型时抛出异常</exception>
        public Entity AddChild(Entity entity)
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            entity.Parent = this;
            return entity;
        }
        
        /// <summary>
        /// 添加指定类型的子实体，并自动初始化
        /// </summary>
        /// <typeparam name="T">子实体类型，必须实现IAwake接口</typeparam>
        /// <returns>创建的子实体实例</returns>
        public T AddChild<T>() where T : Entity, IAwake
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            T component = Create<T>();
            component.Id = GenerateID();
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component);
            return component;
        }
        /// <summary>
        /// 根据ID获取指定类型的子实体
        /// </summary>
        /// <typeparam name="K">子实体类型</typeparam>
        /// <param name="id">子实体ID</param>
        /// <returns>找到的子实体，如果不存在返回null</returns>
        public K GetChild<K>(long id) where K : Entity
        {
            if (this._children == null)
            {
                return null;
            }
            this._children.TryGetValue(id, out Entity child);
            return child as K;
        }

        /// <summary>
        /// 添加带一个参数的子实体
        /// </summary>
        /// <typeparam name="T1">子实体类型</typeparam>
        /// <typeparam name="T2">参数类型</typeparam>
        /// <param name="a">初始化参数</param>
        /// <returns>创建的子实体实例</returns>
        public T1 AddChild<T1, T2>(T2 a) where T1 : Entity, IAwake<T2>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component, a);
            return component;
        }
        
        /// <summary>
        /// 添加带两个参数的子实体
        /// </summary>
        /// <typeparam name="T1">子实体类型</typeparam>
        /// <typeparam name="T2">第一个参数类型</typeparam>
        /// <typeparam name="T3">第二个参数类型</typeparam>
        public T1 AddChild<T1, T2, T3>(T2 a, T3 b) where T1 : Entity, IAwake<T2, T3>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component, a, b);
            return component;
        }
        /// <summary>
        /// 添加带三个参数的子实体
        /// </summary>
        public T1 AddChild<T1, T2, T3, T4>(T2 a, T3 b, T4 c) where T1 : Entity, IAwake<T2, T3, T4>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component, a, b, c);
            return component;
        }
        /// <summary>
        /// 使用指定ID添加子实体
        /// </summary>
        /// <param name="id">指定的实体ID</param>
        /// <param name="isFromPool">是否从对象池创建</param>
        public T AddChildWithId<T>(long id, bool isFromPool = false) where T : Entity, IAwake
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type);
            component.Id = id;
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component);
            return component;
        }
        /// <summary>
        /// 将实体添加到子实体字典中
        /// </summary>
        private void AddToChildren(Entity entity)
        {
            this.Childrens.Add(entity.Id, entity);
        }
        /// <summary>
        /// 使用指定ID和参数添加子实体
        /// </summary>
        public T AddChildWithId<T, A>(long id, A a, bool isFromPool = false) where T : Entity, IAwake<A>
        {
            Type type = typeof(T);
            T component = (T)Entity.Create(type);
            component.Id = id;
            component.Parent = this;
            EntitySystemSingleton.Instance.Awake(component, a);
            return component;
        }
        /// <summary>
        /// 添加组件实体
        /// </summary>
        /// <exception cref="Exception">当组件已存在时抛出异常</exception>
        public Entity AddComponent(Entity component)
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            Type type = component.GetType();
            if (this._component != null && this._component.ContainsKey(this.GetLongHashCode(type)))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }
            component.Parent = this;
            AddComponentChild(component);
           
            return component;
        }
        /// <summary>
        /// 添加指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>创建的组件实例</returns>
        public T AddComponent<T>() where T : Entity, IAwake
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            if (this._component != null && _component.ContainsKey(GetLongHashCode(typeof(T))))
            {
                throw new Exception($"entity already has component: {typeof(T).FullName}");
            }

            T component = Create<T>();
            component.Id = GenerateID();
            component.Parent = this;
            
            AddComponentChild(component);
            EntitySystemSingleton.Instance.Awake(component);
           
            return component;
        }
                public T1 AddComponent<T1, T2>(T2 a) where T1 : Entity, IAwake<T2>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            if (this._component != null && _component.ContainsKey(GetLongHashCode(typeof(T1))))
            {
                throw new Exception($"entity already has component: {typeof(T1).FullName}");
            }

            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            
            AddComponentChild(component);
            EntitySystemSingleton.Instance.Awake(component, a);
           
            return component;
        }
        
        public T1 AddComponent<T1, T2, T3>(T2 a, T3 b) where T1 : Entity, IAwake<T2, T3>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            if (this._component != null && _component.ContainsKey(GetLongHashCode(typeof(T1))))
            {
                throw new Exception($"entity already has component: {typeof(T1).FullName}");
            }

            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            
            AddComponentChild(component);
            EntitySystemSingleton.Instance.Awake(component, a, b);
           
            return component;
        }
        public T1 AddComponent<T1, T2, T3, T4>(T2 a, T3 b, T4 c) where T1 : Entity, IAwake<T2, T3, T4>
        {
            if (!(this is Scene) && Parent == null)
            {
                throw new Exception("Parent IsNull Can't Add Child");
            }
            if (this._component != null && _component.ContainsKey(GetLongHashCode(typeof(T1))))
            {
                throw new Exception($"entity already has component: {typeof(T1).FullName}");
            }

            T1 component = Create<T1>();
            component.Id = GenerateID();
            component.Parent = this;
            
            AddComponentChild(component);
            EntitySystemSingleton.Instance.Awake(component, a, b, c);
           
            return component;
        }
        
        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>找到的组件实例，不存在返回null</returns>
        public T GetComponent<T>() where T : Entity
        {
            if (_component == null)
                return null;
            var longHashCode = GetLongHashCode(typeof(T));
            if(_component.TryGetValue(longHashCode, out var c))
            {
                return c as T;
            }
            return null;
        }
        public Entity GetComponent(Type type)
        {
            if (this._component == null)
            {
                return null;
            }

            // 如果有IGetComponent接口，则触发GetComponentSystem
            // 这个要在tryget之前调用，因为有可能components没有，但是执行GetComponentSystem后又有了
            // if (this is IGetComponentSys)
            // {
            //     EntitySystemSingleton.Instance.GetComponentSys(this, type);
            // }

            Entity component;
            if (!this._component.TryGetValue(this.GetLongHashCode(type), out component))
            {
                return null;
            }

            return component;
        }
        /// <summary>
        /// 获取父实体并转换为指定类型
        /// </summary>
        public T GetParent<T>() where T : Entity
        {
            return this.Parent as T;
        }
        /// <summary>
        /// 移除指定类型的组件
        /// </summary>
        public void RemoveComponent<K>() where K : Entity
        {
            if (_isDisposed)
            {
                return;
            }
            if (_component == null)
            {
                return;
            }
            Type type = typeof (K);
            Entity c;
            if (!_component.TryGetValue(GetLongHashCode(type), out c))
            {
                return;
            }
            _component.Remove(GetLongHashCode(type));
            c.Dispose();
        }
        

        /// <summary>
        /// 移除指定ID的子实体
        /// </summary>
        public void RemoveChild(long id)
        {
            if (_children == null)
            {
                return;
            }
            if (!_children.TryGetValue(id, out Entity child))
            {
                return;
            }
            _children.Remove(id);
            child.Dispose();
        }
        
        /// <summary>
        /// 获取类型的哈希码
        /// </summary>
        long GetLongHashCode(Type type)
        {
            // using var sha256 = SHA256.Create();
            // byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(type.Name));
            // return BitConverter.ToInt64(bytes, 0);
            return type.TypeHandle.Value.ToInt64();
        }
        /// <summary>
        /// 获取类型的稳定哈希码（使用SHA256）
        /// </summary>
        long GetStableLongHashCode(Type type)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(type.FullName));
                return BitConverter.ToInt64(bytes, 0);
            }
        }        
        void RemoveComponent(Entity component)
        {
            if (_isDisposed)
            {
                return;
            }

            if (_component == null)
            {
                return;
            }

            Entity c;
            if (!_component.TryGetValue(GetLongHashCode(_component.GetType()), out c))
            {
                return;
            }
            _component.Remove(GetLongHashCode(component.GetType()));
            c.Dispose();
        }
        
        /// <summary>
        /// 从子实体集合中移除实体
        /// </summary>
        public void RemoveFromChildren(Entity entity)
        {
            if (this._children == null)
            {
                return;
            }

            this._children.Remove(entity.Id);
            
            if (this._children.Count == 0)
            {
                this._children = null;
            }
        }
        /// <summary>
        /// 销毁实体，清理所有子实体和组件
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed)
                return;
            _isDisposed = true;
            this._isRegister = false;
            // 清理 _children
            if (_children != null)
            {
                foreach (var e in _children.Values)
                {
                    e.Dispose();
                }
                this._children.Clear();
                this._children = null;
            }
            // 清理 Component 
            if (_component != null)
            {
                foreach (var e in _component.Values)
                {
                    e.Dispose();
                }
                this._component.Clear();
                this._component = null;
            }
            // 触发destroy
            if (this is IDestroy)
            {
                EntitySystemSingleton.Instance.Destroy(this);
            }
            // 清理parent
            if (this._parent != null && !this._parent._isDisposed)
            {
                if (_isComponent)
                {
                    _parent.RemoveComponent(this);
                }
                else
                {
                    _parent.RemoveFromChildren(this);
                }
            }
            this._parent = null;
        }
        /// <summary>
        /// 开始序列化初始化
        /// </summary>
        public void BeginInit()
        {
            // EntitySystemSingleton.Instance.Serialize(this);
            this._componentDB?.Clear();
            if (this._component != null && this._component.Count != 0)
            {
                foreach (Entity entity in this._component.Values)
                {
                    // var type = entity.GetType();
                    // var isDefined = type.IsDefined(typeof(SaveAttribute), true);
                    // if (!isDefined)
                    //     continue;
                    if (entity is not ISerializeToEntity)
                    {
                        continue;
                    }
                    this._componentDB ??= new List<Entity>();
                    this._componentDB.Add(entity);
                    entity.BeginInit();
                }
            }

            this._childrenDB?.Clear();
            if (this._children != null && this._children.Count != 0)
            {
                foreach (Entity entity in this._children.Values)
                {
                    if (entity is not ISerializeToEntity)
                    {
                        continue;
                    }
                    this._childrenDB ??= new List<Entity>();
                    this._childrenDB.Add(entity);
                    entity.BeginInit();
                }
            }
        }
    }
}