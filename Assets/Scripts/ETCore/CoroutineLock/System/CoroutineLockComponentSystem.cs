using UnityEngine;

namespace MH
{
    /// <summary>
    /// 协程锁组件的唤醒系统
    /// 负责初始化协程锁组件
    /// </summary>
    [EntitySystem]
    public class CoroutineLockComponentAwakeSystem : AwakeSystem<CoroutineLockComponent>
    {
        /// <summary>
        /// 初始化协程锁组件
        /// </summary>
        /// <param name="self">协程锁组件实例</param>
        protected override void Awake(CoroutineLockComponent self)
        {
            
        }
    }

    /// <summary>
    /// 协程锁组件的更新系统
    /// 负责处理下一帧需要执行的协程锁队列
    /// </summary>
    [EntitySystem]
    public class CoroutineLockComponentUpdateSystem : UpdateSystem<CoroutineLockComponent>
    {
        /// <summary>
        /// 处理下一帧需要执行的协程锁
        /// </summary>
        /// <param name="self">协程锁组件实例</param>
        protected override void Update(CoroutineLockComponent self)
        {
            while (self.nextFrameRun.Count > 0)
            {
                (int coroutineLockType, long key, int count) = self.nextFrameRun.Dequeue();
                self.Notify(coroutineLockType, key, count);
            }
        }
    }

    /// <summary>
    /// 协程锁组件的扩展方法类
    /// 提供协程锁的等待、通知和运行控制功能
    /// </summary>
    public static class CoroutineLockComponentSystem
    {
        /// <summary>
        /// 将协程添加到下一帧执行队列
        /// </summary>
        /// <param name="self">协程锁组件实例</param>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁键值</param>
        /// <param name="level">协程执行层级</param>
        public static void RunNextCoroutine(this CoroutineLockComponent self, int coroutineLockType, long key, int level)
        {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100)
            {
                Debug.LogWarning($"too much coroutine level: {coroutineLockType} {key}");
            }

            self.nextFrameRun.Enqueue((coroutineLockType, key, level));
        }

        /// <summary>
        /// 等待获取协程锁
        /// </summary>
        /// <param name="self">协程锁组件实例</param>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁键值</param>
        /// <param name="time">等待超时时间(毫秒)</param>
        /// <returns>协程锁实例</returns>
        public static async ETTask<CoroutineLock> Wait(this CoroutineLockComponent self, int coroutineLockType, long key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = self.GetChild<CoroutineLockQueueType>(coroutineLockType) ?? self.AddChildWithId<CoroutineLockQueueType>(coroutineLockType);
            return await coroutineLockQueueType.Wait(key, time);
        }

        /// <summary>
        /// 通知指定类型和键值的协程锁
        /// </summary>
        /// <param name="self">协程锁组件实例</param>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁键值</param>
        /// <param name="level">协程执行层级</param>
        public static void Notify(this CoroutineLockComponent self, int coroutineLockType, long key, int level)
        {
            CoroutineLockQueueType coroutineLockQueueType = self.GetChild<CoroutineLockQueueType>(coroutineLockType);
            if (coroutineLockQueueType == null)
            {
                return;
            }
            coroutineLockQueueType.Notify(key, level);
        }
    }
}