using System.Collections.Generic;
using UnityEngine;

namespace MH
{
    [Invoke(TimerInvokeType.BuffActionIntervalTimerInvoke)]
    public class Buff_TimerAction : ATimer<BuffInfo>
    {
        protected override void Run(BuffInfo self)
        {
            self.TryStartCountDown();
        }
    }
    [EntitySystem]
    public class BuffInfoAwakeSystem: AwakeSystem<BuffInfo, int, Unit, Unit>
    {
        protected override void Awake(BuffInfo self, int configId, Unit caster, Unit target)
        {
            self.ConfigId = configId;
            self.CreateTime = TimeInfo.Instance.ClientNow();
            self.EndTime = self.Config.BuffInfoConfig.BuffInfoTimer * 1000 + self.CreateTime;
            self.Caster = caster;
            self.Target = target;
            self.Layer = 1;
            BuffActionDispatcher.Instance.OnStart(self);
            self.TryStartCountDown();
        }
    }
    
    [EntitySystem]
    public class BuffInfoDestroySystem: DestroySystem<BuffInfo>
    {
        protected override void Destroy(BuffInfo self)
        {
            BuffActionDispatcher.Instance.OnEnd(self);
            self.EndTime = 0;
            self.ConfigId = 0;
            self.Target = null;
            self.Caster = null;
            self.Layer = 0;
            self.Root.GetComponent<TimerComponent>().Remove(ref self.TimerId);
        }
    }

    public static class BuffInfoSystem
    {
        /// <summary>
        /// 重复添加buff
        /// </summary>
        /// <param name="self"></param>
        public static void RepeatAddBuffInfo(this BuffInfo self)
        {
            int oldLayer = self.Layer;
            switch (self.Config.BuffInfoConfig.BuffAddType)
            {
                case (int)BuffAddType.ResetTime:
                    self.EndTime = self.Config.BuffInfoConfig.BuffInfoTimer * 1000 + TimeInfo.Instance.ClientNow();
                    break;
                case (int)BuffAddType.MultipleLayer:
                    if (self.Layer < self.Config.BuffInfoConfig.MaxLayer)
                        self.Layer++;
                    break;
                case (int)BuffAddType.MultipleLayerAndResetTime:
                    if (self.Layer < self.Config.BuffInfoConfig.MaxLayer)
                        self.Layer++;
                    self.EndTime = self.Config.BuffInfoConfig.BuffInfoTimer * 1000 + TimeInfo.Instance.ClientNow();
                    break;
            }
            //如果层数发生变化，则调用BuffActionDispatcher的OnLevelChange方法
            if (oldLayer != self.Layer)
                BuffActionDispatcher.Instance.OnLevelChange(self, oldLayer, self.Layer);
        }
        /// <summary>
        /// 尝试开始计时器
        /// </summary>
        /// <param name="self"></param>
        public static void TryStartCountDown(this BuffInfo self)
        {
            if (!self.TryCompleted())
            {
                var timerComponent = self.Root.GetComponent<TimerComponent>();
                self.TimerId = timerComponent.NewOnceTimer(TimeInfo.Instance.ClientNow() + (int)(self.Config.BuffInfoConfig.Interval * 1000f),
                        TimerInvokeType.BuffActionIntervalTimerInvoke, self);
                BuffActionDispatcher.Instance.Run(self);
                return;
            }
            //如果buff完成，则移除buff
            BuffFactory.RemoveBuff(self);
        }
        /// <summary>
        /// 尝试完成buff
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool TryCompleted(this BuffInfo self)
        {
            int oldLayer = self.Layer;
            if (self.EndTime > TimeInfo.Instance.ClientNow())
                return false;
            //还有层数
            if (self.Config.BuffInfoConfig.CycleEndType == (int)CycleEndType.Clear)
                self.Layer = 0;
            else if (self.Config.BuffInfoConfig.CycleEndType == (int)CycleEndType.Sub)
            {
                self.Layer -= 1;
                self.EndTime = self.Config.BuffInfoConfig.BuffInfoTimer * 1000 + TimeInfo.Instance.ClientNow();
            }
            //销毁
            if (self.Layer > 0)
                return false;
            //如果层数发生变化，则调用BuffActionDispatcher的OnLevelChange方法
            if (oldLayer != self.Layer)
                BuffActionDispatcher.Instance.OnLevelChange(self, oldLayer, self.Layer);
            return true;
        }
    }
}