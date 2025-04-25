using System.Collections.Generic;
using System;
using UnityEngine;

namespace MH
{
    public class EventSystem : LogicSingleton<EventSystem>, ISingletonAwake
    {
        private class EventInfo
        {
            public IEvent IEvent { get; }

            public SceneType SceneType { get; }

            public EventInfo(IEvent iEvent, SceneType sceneType)
            {
                this.IEvent = iEvent;
                this.SceneType = sceneType;
            }
        }
        private Dictionary<Type, List<EventInfo>> _allEvents = new Dictionary<Type, List<EventInfo>>();
        private readonly Dictionary<Type, Dictionary<long, object>> allInvokers = new();
        /// <summary>
        /// 初始化事件系统
        /// 扫描所有继承自IEvent的类，并注册它们的事件处理程序
        /// </summary>
        public void Awake()
        {
            foreach (Type type in CodeTypes.Instance.GetTypes(typeof(EventAttribute)))
            {
                IEvent obj = Activator.CreateInstance(type) as IEvent;
                if (obj == null)
                {
                    throw new Exception($"type not is AEvent: {type.FullName}");
                }
                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);
                foreach (object attr in attrs)
                {
                    EventAttribute eventAttribute = attr as EventAttribute;

                    Type eventType = obj.Type;

                    EventInfo eventInfo = new(obj, eventAttribute.SceneType);

                    if (!this._allEvents.ContainsKey(eventType))
                    {
                        this._allEvents.Add(eventType, new List<EventInfo>());
                    }
                    this._allEvents[eventType].Add(eventInfo);
                }
            }
            foreach (Type type in CodeTypes.Instance.GetTypes(typeof(InvokeAttribute)))
            {
                object obj = Activator.CreateInstance(type);
                IInvoke iInvoke = obj as IInvoke;
                if (iInvoke == null)
                {
                    throw new Exception($"type not is callback: {type.Name}");
                }

                object[] attrs = type.GetCustomAttributes(typeof(InvokeAttribute), false);
                foreach (object attr in attrs)
                {
                    if (!this.allInvokers.TryGetValue(iInvoke.Type, out var dict))
                    {
                        dict = new Dictionary<long, object>();
                        this.allInvokers.Add(iInvoke.Type, dict);
                    }

                    InvokeAttribute invokeAttribute = attr as InvokeAttribute;

                    try
                    {
                        dict.Add(invokeAttribute.Type, obj);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"action type duplicate: {iInvoke.Type.Name} {invokeAttribute.Type}", e);
                    }
                }
            }
        }

        /// <summary>
        /// 异步发布事件
        /// 在指定场景中发布事件，并等待所有事件处理完成
        /// </summary>
        /// <typeparam name="S">事件场景类型</typeparam>
        /// <typeparam name="T">事件参数类型</typeparam>
        public async ETTask PublishAsync<S, T>(S scene, T args) where S : Scene where T : struct
        {
            List<EventInfo> iEvents;
            if (!this._allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return;
            }

            List<ETTask> list = new List<ETTask>();
            foreach (var eventInfo in iEvents)
            {
                if (!scene.SceneType.HasSameFlag(eventInfo.SceneType))
                    continue;
                if (!(eventInfo.IEvent is AEvent<S, T> aEvent))
                {
                    Debug.LogError($"event error: {eventInfo.IEvent.GetType().FullName}");
                    continue;
                }
                list.Add(aEvent.Handle(scene, args));
            }
            try
            {
                await ETTaskHelper.WaitAll(list);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        /// <summary>
        /// 发布事件
        /// 在指定场景中发布事件，并等待所有事件处理完成
        /// </summary>
        /// <typeparam name="S">事件场景类型</typeparam>
        /// <typeparam name="T">事件参数类型</typeparam>
        public void Publish<S, T>(S scene, T args) where S : Scene where T : struct
        {
            List<EventInfo> iEvents;
            if (!this._allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return;
            }
            SceneType sceneType = scene.SceneType;
            foreach (var eventInfo in iEvents)
            {
                if (!sceneType.HasSameFlag(eventInfo.SceneType))
                    continue;
                if (!(eventInfo.IEvent is AEvent<S, T> aEvent))
                {
                    Debug.LogError($"event error: {eventInfo.IEvent.GetType().FullName}");
                    continue;
                }
                aEvent.Handle(scene, args).Coroutine();
            }
        }
        // Invoke跟Publish的区别(特别注意)
        // Invoke类似函数，必须有被调用方，否则异常，调用者跟被调用者属于同一模块，比如MoveComponent中的Timer计时器，调用跟被调用的代码均属于移动模块
        // 既然Invoke跟函数一样，那么为什么不使用函数呢? 因为有时候不方便直接调用，比如Config加载，在客户端跟服务端加载方式不一样。比如TimerComponent需要根据Id分发
        // 注意，不要把Invoke当函数使用，这样会造成代码可读性降低，能用函数不要用Invoke
        // publish是事件，抛出去可以没人订阅，调用者跟被调用者属于两个模块，比如任务系统需要知道道具使用的信息，则订阅道具使用事件
        public void Invoke<A>(long type, A args) where A : struct
        {
            if (!this.allInvokers.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error1: {type} {typeof(A).FullName}");
            }
            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error2: {type} {typeof(A).FullName}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error3, not AInvokeHandler: {type} {typeof(A).FullName}");
            }

            aInvokeHandler.Handle(args);
        }
        /// <summary>
        /// 异步调用事件
        /// 在指定场景中发布事件，并等待所有事件处理完成
        /// </summary>
        /// <typeparam name="A">事件参数类型</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        public T Invoke<A, T>(long type, A args) where A : struct
        {
            if (!this.allInvokers.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error4: {type} {typeof(A).FullName}");
            }

            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error5: {type} {typeof(A).FullName}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A, T>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error6, not AInvokeHandler: {type} {typeof(A).FullName} {typeof(T).FullName} ");
            }

            return aInvokeHandler.Handle(args);
        }
        /// <summary>
        /// 异步调用事件
        /// 在指定场景中发布事件，并等待所有事件处理完成
        /// </summary>
        /// <typeparam name="A">事件参数类型</typeparam>
        public void Invoke<A>(A args) where A : struct
        {
            Invoke(0, args);
        }
        /// <summary>
        /// 异步调用事件
        /// 在指定场景中发布事件，并等待所有事件处理完成
        /// </summary>
        /// <typeparam name="A">事件参数类型</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        public T Invoke<A, T>(A args) where A : struct
        {
            return Invoke<A, T>(0, args);
        }
    }
}