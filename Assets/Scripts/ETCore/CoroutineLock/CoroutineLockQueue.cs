using System;
using System.Collections.Generic;

namespace MH
{
    /// <summary>
    /// 协程锁队列的唤醒系统
    /// 负责初始化协程锁队列的基本属性
    /// </summary>
    [EntitySystem]
    public class CoroutineLockQueueAwakeSystem : AwakeSystem<CoroutineLockQueue, int>
    {
        /// <summary>
        /// 初始化协程锁队列
        /// </summary>
        /// <param name="self">协程锁队列实例</param>
        /// <param name="type">协程锁类型</param>
        protected override void Awake(CoroutineLockQueue self, int type)
        {
            self.isStart = false;
            self.type = type;
        }
    }

    /// <summary>
    /// 协程锁队列的销毁系统
    /// 负责清理协程锁队列的资源
    /// </summary>
    [EntitySystem]
    public class CoroutineLockQueueDestroySystem : DestroySystem<CoroutineLockQueue>
    {
        /// <summary>
        /// 销毁协程锁队列
        /// 清理队列并重置状态
        /// </summary>
        /// <param name="self">协程锁队列实例</param>
        protected override void Destroy(CoroutineLockQueue self)
        {
            self.queue.Clear();
            self.type = 0;
            self.isStart = false;
        }
    }

    /// <summary>
    /// 协程锁队列的扩展方法类
    /// 提供等待和通知功能
    /// </summary>
    public static class CoroutineLockQueueSystem
    {
        /// <summary>
        /// 等待获取协程锁
        /// 如果队列未启动，直接创建新的协程锁
        /// 否则将等待请求加入队列
        /// </summary>
        /// <param name="self">协程锁队列实例</param>
        /// <param name="time">等待超时时间(毫秒)</param>
        /// <returns>协程锁实例</returns>
        public static async ETTask<CoroutineLock> Wait(this CoroutineLockQueue self, int time)
        {
            CoroutineLock coroutineLock = null;
            if (!self.isStart)
            {
                self.isStart = true;
                coroutineLock = self.AddChild<CoroutineLock, int, long, int>(self.type, self.Id, 1);
                return coroutineLock;
            }

            WaitCoroutineLock waitCoroutineLock = WaitCoroutineLock.Create();
            self.queue.Enqueue(waitCoroutineLock);
            if (time > 0)
            {
                long tillTime = TimeInfo.Instance.ClientNow() + time;
                self.Root.GetComponent<TimerComponent>().NewOnceTimer(tillTime, TimerInvokeType.CoroutineTimeout, waitCoroutineLock);
            }
            coroutineLock = await waitCoroutineLock.Wait();
            return coroutineLock;
        }

        /// <summary>
        /// 通知等待队列中的下一个协程锁
        /// 找到一个未处理的等待请求并创建新的协程锁
        /// </summary>
        /// <param name="self">协程锁队列实例</param>
        /// <param name="level">协程执行层级</param>
        /// <returns>是否找到并处理了一个有效的等待请求</returns>
        public static bool Notify(this CoroutineLockQueue self, int level)
        {
            // 有可能WaitCoroutineLock已经超时抛出异常，所以要找到一个未处理的WaitCoroutineLock
            while (self.queue.Count > 0)
            {
                WaitCoroutineLock waitCoroutineLock = self.queue.Dequeue();

                if (waitCoroutineLock.IsDisposed())
                {
                    continue;
                }

                CoroutineLock coroutineLock = self.AddChild<CoroutineLock, int, long, int>(self.type, self.Id, level);

                waitCoroutineLock.SetResult(coroutineLock);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 协程锁队列类
    /// 管理一组相同类型的协程锁的等待队列
    /// </summary>
    public class CoroutineLockQueue: Entity, IAwake<int>, IDestroy
    {
        /// <summary>
        /// 协程锁类型
        /// </summary>
        public int type;

        /// <summary>
        /// 队列是否已启动
        /// </summary>
        public bool isStart;
        
        /// <summary>
        /// 等待协程锁的队列
        /// </summary>
        public Queue<WaitCoroutineLock> queue = new();

        /// <summary>
        /// 获取队列中等待的协程锁数量
        /// </summary>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }
    }
}