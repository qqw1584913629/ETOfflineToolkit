using System.Collections.Generic;

namespace MH
{
    /// <summary>
    /// 协程锁组件类
    /// 负责管理协程锁的执行队列和调度
    /// 实现了唤醒和更新接口
    /// </summary>
    public class CoroutineLockComponent : Entity, IAwake, IUpdate
    {
        /// <summary>
        /// 下一帧需要执行的协程队列
        /// 元组包含：(协程锁类型, 键值, 执行层级)
        /// </summary>
        public readonly Queue<(int, long, int)> nextFrameRun = new();
    }
}