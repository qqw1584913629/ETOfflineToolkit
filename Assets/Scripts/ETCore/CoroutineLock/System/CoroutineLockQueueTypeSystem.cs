namespace MH
{
    /// <summary>
    /// 协程锁队列类型的唤醒系统
    /// 负责初始化协程锁队列类型组件
    /// </summary>
    [EntitySystem]
    public class CoroutineLockQueueTypeAwakeSystem : AwakeSystem<CoroutineLockQueueType>
    {
        /// <summary>
        /// 初始化协程锁队列类型组件
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        protected override void Awake(CoroutineLockQueueType self)
        {
            
        }
    }

    /// <summary>
    /// 协程锁队列类型的扩展方法类
    /// 提供协程锁队列的创建、获取、移除和等待功能
    /// </summary>
    public static class CoroutineLockQueueTypeSystem
    {
        /// <summary>
        /// 获取指定键值的协程锁队列
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        /// <param name="key">队列键值</param>
        /// <returns>协程锁队列实例，不存在则返回null</returns>
        public static CoroutineLockQueue Get(this CoroutineLockQueueType self, long key)
        {
            return self.GetChild<CoroutineLockQueue>(key);
        }

        /// <summary>
        /// 创建新的协程锁队列
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        /// <param name="key">队列键值</param>
        /// <returns>新创建的协程锁队列实例</returns>
        public static CoroutineLockQueue New(this CoroutineLockQueueType self, long key)
        {
            CoroutineLockQueue queue = self.AddChildWithId<CoroutineLockQueue, int>(key, (int)self.Id, true);
            return queue;
        }

        /// <summary>
        /// 移除指定键值的协程锁队列
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        /// <param name="key">要移除的队列键值</param>
        public static void Remove(this CoroutineLockQueueType self, long key)
        {
            self.RemoveChild(key);
        }

        /// <summary>
        /// 等待获取协程锁
        /// 如果队列不存在则创建新队列
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        /// <param name="key">队列键值</param>
        /// <param name="time">等待超时时间(毫秒)</param>
        /// <returns>协程锁实例</returns>
        public static async ETTask<CoroutineLock> Wait(this CoroutineLockQueueType self, long key, int time)
        {
            CoroutineLockQueue queue = self.Get(key) ?? self.New(key);
            return await queue.Wait(time);
        }

        /// <summary>
        /// 通知指定键值的协程锁队列
        /// 如果通知后队列为空则移除该队列
        /// </summary>
        /// <param name="self">协程锁队列类型实例</param>
        /// <param name="key">队列键值</param>
        /// <param name="level">协程执行层级</param>
        public static void Notify(this CoroutineLockQueueType self, long key, int level)
        {
            CoroutineLockQueue queue = self.Get(key);
            if (queue == null)
            {
                return;
            }

            if (!queue.Notify(level))
            {
                self.Remove(key);
            }
        }
    }
}