using UnityEngine;

namespace MH
{
    
    [EntitySystem]
    public class TimerComponentAwakeSystem: AwakeSystem<TimerComponent>
    {
        protected override void Awake(TimerComponent self)
        {
            
        }
    }
    [EntitySystem]
    public class TimerComponentUpdateSystem : UpdateSystem<TimerComponent>
    {
        /// <summary>
        /// 更新定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        protected override void Update(TimerComponent self)
        {
            if (self.timeId.Count == 0)
            {
                return;
            }

            long timeNow = self.GetNow();

            if (timeNow < self.minTime)
            {
                return;
            }

            foreach (var kv in self.timeId)
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    self.minTime = k;
                    break;
                }

                self.timeOutTime.Enqueue(k);
            }

            while (self.timeOutTime.Count > 0)
            {
                long time = self.timeOutTime.Dequeue();
                var list = self.timeId[time];
                for (int i = 0; i < list.Length; ++i)
                {
                    long timerId = list[i];
                    self.timeOutTimerIds.Enqueue(timerId);
                }
                self.timeId.Remove(time);
            }

            if (self.timeId.Count == 0)
            {
                self.minTime = long.MaxValue;
            }

            while (self.timeOutTimerIds.Count > 0)
            {
                long timerId = self.timeOutTimerIds.Dequeue();

                if (!self.timerActions.Remove(timerId, out TimerAction timerAction))
                {
                    continue;
                }

                self.Run(timerId, ref timerAction);
            }
        }
    }
    public struct TimerCallback
    {
        public object Args;
    }
    public static class TimerComponentSystem
    {
        /// <summary>
        /// 获取ID
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <returns>ID</returns>
        private static long GetId(this TimerComponent self)
        {
            return ++self.idGenerator;
        }
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <returns>当前时间</returns>
        public static long GetNow(this TimerComponent self)
        {
            return TimeInfo.Instance.ClientNow();
        }
        /// <summary>
        /// 运行定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="timerId">定时器ID</param>
        /// <param name="timerAction">定时器动作</param>        
        public static void Run(this TimerComponent self, long timerId, ref TimerAction timerAction)
        {
            switch (timerAction.TimerClass)
            {
                case TimerClass.OnceTimer:
                {
                    EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                    break;
                }
                case TimerClass.OnceWaitTimer:
                {
                    ETTask tcs = timerAction.Object as ETTask;
                    tcs?.SetResult();
                    break;
                }
                case TimerClass.RepeatedTimer:
                {
                    long timeNow = self.GetNow();
                    timerAction.StartTime = timeNow;
                    self.AddTimer(timerId, ref timerAction);
                    EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                    break;
                }
            }
        }
        /// <summary>
        /// 添加定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="timerId">定时器ID</param>
        /// <param name="timer">定时器</param>
        private static void AddTimer(this TimerComponent self, long timerId, ref TimerAction timer)
        {
            long tillTime = timer.StartTime + timer.Time;
            self.timeId.Add(tillTime, timerId);
            self.timerActions.Add(timerId, timer);
            if (tillTime < self.minTime)
            {
                self.minTime = tillTime;
            }
        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="id">定时器ID</param>
        /// <returns>是否移除成功</returns>
        public static bool Remove(this TimerComponent self, ref long id)
        {
            long i = id;
            id = 0;
            return self.Remove(i);
        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="id">定时器ID</param>
        /// <returns>是否移除成功</returns>
        private static bool Remove(this TimerComponent self, long id)
        {
            if (id == 0)
            {
                return false;
            }

            if (!self.timerActions.Remove(id, out TimerAction _))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 等待定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="tillTime">定时器时间</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async ETTask WaitTillAsync(this TimerComponent self, long tillTime, ETCancellationToken cancellationToken = null)
        {
            long timeNow = self.GetNow();
            if (timeNow >= tillTime)
            {
                return;
            }

            ETTask tcs = ETTask.Create();
            long timerId = self.GetId();
            TimerAction timer = new(TimerClass.OnceWaitTimer, timeNow, tillTime - timeNow, 0, tcs);
            self.AddTimer(timerId, ref timer);

            void CancelAction()
            {
                if (self.Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }
        /// <summary>
        /// 等待帧
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async ETTask WaitFrameAsync(this TimerComponent self, ETCancellationToken cancellationToken = null)
        {
            await self.WaitAsync(1, cancellationToken);
        }
        /// <summary>
        /// 等待定时器
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="time">时间</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async ETTask WaitAsync(this TimerComponent self, long time, ETCancellationToken cancellationToken = null)
        {
            if (time == 0)
            {
                return;
            }

            long timeNow = self.GetNow();

            ETTask tcs = ETTask.Create();
            long timerId = self.GetId();
            TimerAction timer = new(TimerClass.OnceWaitTimer, timeNow, time, 0, tcs);
            self.AddTimer(timerId, ref timer);

            void CancelAction()
            {
                if (self.Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }

        // 用这个优点是可以热更，缺点是回调式的写法，逻辑不连贯。WaitTillAsync不能热更，优点是逻辑连贯。
        // wait时间短并且逻辑需要连贯的建议WaitTillAsync
        // wait时间长不需要逻辑连贯的建议用NewOnceTimer
        public static long NewOnceTimer(this TimerComponent self, long tillTime, int type, object args)
        {
            long timeNow = self.GetNow();
            if (tillTime < timeNow)
            {
                Debug.LogError($"new once time too small: {tillTime}");
            }
            long timerId = self.GetId();
            TimerAction timer = new(TimerClass.OnceTimer, timeNow, tillTime - timeNow, type, args);
            self.AddTimer(timerId, ref timer);
            return timerId;
        }
        /// <summary>
        /// 创建一个FrameTimer
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        /// <returns>定时器ID</returns>
        public static long NewFrameTimer(this TimerComponent self, int type, object args)
        {
            return self.NewRepeatedTimerInner(0, type, args);
        }

        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        private static long NewRepeatedTimerInner(this TimerComponent self, long time, int type, object args)
        {
            long timeNow = self.GetNow();
            long timerId = self.GetId();
            TimerAction timer = new(TimerClass.RepeatedTimer, timeNow, time, type, args);

            // 每帧执行的不用加到timerId中，防止遍历
            self.AddTimer(timerId, ref timer);
            return timerId;
        }
        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        /// <param name="self">定时器组件</param>
        /// <param name="time">时间</param>
        /// <param name="type">类型</param>
        /// <param name="args">参数</param>
        public static long NewRepeatedTimer(this TimerComponent self, long time, int type, object args)
        {
            if (time < 100)
            {
                Debug.LogError($"time too small: {time}");
                return 0;
            }

            return self.NewRepeatedTimerInner(time, type, args);
        }
    }
}