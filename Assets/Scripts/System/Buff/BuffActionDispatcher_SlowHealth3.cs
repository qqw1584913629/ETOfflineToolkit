using System;
using UnityEngine;
namespace MH
{
    /// <summary>
    /// 每秒降低3%的最大生命值，持续3秒，最高叠加1层
    /// </summary>
    [BuffAction(BuffInfoType.SlowHealth3)]
    public class BuffActionDispatcher_SlowHealth3 : IBuffAction
    {
        public void OnEnd(BuffInfo buffInfo)
        {
            buffInfo.Target.GetComponent<AttributeModifierComponent>()?.ClearSourceModifiers(buffInfo.Id);
        }

        public void OnLevelChange(BuffInfo buffInfo, int oldLayer, int newLayer)
        {
        }

        public void OnStart(BuffInfo buffInfo)
        {
        }

        public void Run(BuffInfo buffInfo)
        {
            var numericComponent = buffInfo.Target.GetComponent<NumericComponent>();
            //首先获取目标的数值组件拿到最大生命值
            var maxHp = numericComponent[NumericType.MaxHp];
            for (int i = 0; i < buffInfo.Config.BuffInfoConfig.NumericTypes.Count; i++)
            {
                var key = buffInfo.Config.BuffInfoConfig.NumericTypes[i];
                var value = buffInfo.Config.BuffInfoConfig.NumericValues[i];
                EventSystem.Instance.PublishAsync(buffInfo.Root, new AttackUnitStartSpecifyDamage()
                {
                    TargetUnit = buffInfo.Target,
                    Damage = (long)(maxHp * value / 1000000f),
                }).Coroutine();
            }

        }
    }
}
