namespace MH
{
    /// <summary>
    /// 协程锁的唤醒系统
    /// 负责初始化单个协程锁实例的基本属性
    /// </summary>
    [EntitySystem]
    public class CoroutineLockAwakeSystem : AwakeSystem<CoroutineLock, int, long, int>
    {
        /// <summary>
        /// 初始化协程锁实例
        /// </summary>
        /// <param name="self">协程锁实例</param>
        /// <param name="type">协程锁类型</param>
        /// <param name="k">协程锁键值</param>
        /// <param name="count">协程执行层级</param>
        protected override void Awake(CoroutineLock self, int type, long k, int count)
        {
            self.type = type;
            self.key = k;
            self.level = count;
        }
    }

    /// <summary>
    /// 协程锁的销毁系统
    /// 负责清理协程锁实例并触发下一个协程的执行
    /// </summary>
    [EntitySystem]
    public class CoroutineLockDestroySystem : DestroySystem<CoroutineLock>
    {
        /// <summary>
        /// 销毁协程锁实例
        /// 触发下一个协程的执行并重置状态
        /// </summary>
        /// <param name="self">协程锁实例</param>
        protected override void Destroy(CoroutineLock self)
        {
            self.Root.GetComponent<CoroutineLockComponent>().RunNextCoroutine(self.type, self.key, self.level + 1);
            self.type = CoroutineLockType.None;
            self.key = 0;
            self.level = 0;
        }
    }

    /// <summary>
    /// 协程锁的扩展方法类
    /// 提供协程锁的相关功能扩展
    /// </summary>
    public static class CoroutineLockSystem
    {
        
    }
}