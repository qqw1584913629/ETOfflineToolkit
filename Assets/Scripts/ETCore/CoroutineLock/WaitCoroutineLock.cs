using System;

namespace MH
{
    /// <summary>
    /// 协程锁等待超时处理器
    /// 当等待协程锁超时时触发异常
    /// </summary>
    [Invoke(TimerInvokeType.CoroutineTimeout)]
    public class WaitCoroutineLockTimer : ATimer<WaitCoroutineLock>
    {
        /// <summary>
        /// 处理协程锁等待超时
        /// 如果等待对象未释放则设置超时异常
        /// </summary>
        /// <param name="waitCoroutineLock">等待的协程锁对象</param>
        protected override void Run(WaitCoroutineLock waitCoroutineLock)
        {
            if (waitCoroutineLock.IsDisposed())
            {
                return;
            }
            waitCoroutineLock.SetException(new Exception("coroutine is timeout!"));
        }
    }

    /// <summary>
    /// 协程锁等待类
    /// 用于异步等待协程锁的获取
    /// 支持超时处理和异常处理
    /// </summary>
    public class WaitCoroutineLock
    {
        /// <summary>
        /// 创建协程锁等待实例
        /// </summary>
        /// <returns>新创建的协程锁等待对象</returns>
        public static WaitCoroutineLock Create()
        {
            WaitCoroutineLock waitCoroutineLock = new WaitCoroutineLock();
            waitCoroutineLock.tcs = ETTask<CoroutineLock>.Create();
            return waitCoroutineLock;
        }

        /// <summary>
        /// 任务完成源
        /// 用于异步等待协程锁的获取
        /// </summary>
        private ETTask<CoroutineLock> tcs;

        /// <summary>
        /// 设置等待结果
        /// 完成等待并返回协程锁
        /// </summary>
        /// <param name="coroutineLock">获取到的协程锁</param>
        /// <exception cref="NullReferenceException">如果任务完成源为空</exception>
        public void SetResult(CoroutineLock coroutineLock)
        {
            if (this.tcs == null)
            {
                throw new NullReferenceException("SetResult tcs is null");
            }
            var t = this.tcs;
            this.tcs = null;
            t.SetResult(coroutineLock);
        }

        /// <summary>
        /// 设置等待异常
        /// 当等待过程出现错误时调用
        /// </summary>
        /// <param name="exception">发生的异常</param>
        /// <exception cref="NullReferenceException">如果任务完成源为空</exception>
        public void SetException(Exception exception)
        {
            if (this.tcs == null)
            {
                throw new NullReferenceException("SetException tcs is null");
            }
            var t = this.tcs;
            this.tcs = null;
            t.SetException(exception);
        }

        /// <summary>
        /// 检查等待对象是否已释放
        /// </summary>
        /// <returns>如果任务完成源为空则返回true</returns>
        public bool IsDisposed()
        {
            return this.tcs == null;
        }

        /// <summary>
        /// 异步等待协程锁
        /// </summary>
        /// <returns>获取到的协程锁</returns>
        public async ETTask<CoroutineLock> Wait()
        {
            return await this.tcs;
        }
    }
}