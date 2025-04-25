namespace MH
{
    /// <summary>
    /// 协程锁实体类
    /// 用于管理和控制协程的执行顺序
    /// 实现了唤醒和销毁接口
    /// </summary>
    public class CoroutineLock : Entity, IAwake<int, long, int>, IDestroy
    {
        /// <summary>
        /// 协程锁类型
        /// 用于区分不同类型的协程锁
        /// </summary>
        public int type;

        /// <summary>
        /// 协程锁键值
        /// 用于唯一标识一个协程锁实例
        /// </summary>
        public long key;

        /// <summary>
        /// 协程执行层级
        /// 用于控制协程的执行顺序和优先级
        /// </summary>
        public int level;
    }
}