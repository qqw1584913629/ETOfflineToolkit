using System;
using System.Collections.Generic;
using System.Linq;

namespace MH
{
    [EntitySystem]
    public class AttributeModifierComponentAwakeSystem : AwakeSystem<AttributeModifierComponent>
    {
        protected override void Awake(AttributeModifierComponent self)
        {

        }
    }
    [EntitySystem]
    public class AttributeModifierComponentDestroySystem : DestroySystem<AttributeModifierComponent>
    {
        protected override void Destroy(AttributeModifierComponent self)
        {
            self.Modifiers.Clear();
        }
    }

    public static class AttributeModifierComponentSystem
    {
        /// <summary>
        /// 添加修改器
        /// </summary>
        /// <param name="self">修改器组件</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="value">修改值</param>
        /// <param name="modifierType">修改器类型</param>
        public static void AddModifier(this AttributeModifierComponent self, int numericType, long sourceId, long value, ModifierType modifierType)
        {
            if(modifierType == ModifierType.Add)
            {
                self.AddAccumulativeModifier(numericType, sourceId, value);
                return;
            }
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();
            if (numericComponent == null)
                return;
            if (!self.Modifiers.TryGetValue(numericType, out var sources))
            {
                sources = new Dictionary<long, long>();
                self.Modifiers[numericType] = sources;
            }
            //try add sourceId KEY
            if (!sources.TryGetValue(sourceId, out long oldValue))
            {
                oldValue = 0;
                sources.Add(sourceId, value);
            }
            else
            {
                sources[sourceId] = value;
            }
            self.ApplyAccumulativeModifier(numericType, -oldValue);
            self.ApplyAccumulativeModifier(numericType, value);

        }
        /// <summary>
        /// 是否存在源
        /// </summary>
        /// <param name="self">修改器组件</param>
        /// <param name="sourceId">来源ID</param>
        /// <returns>是否存在源</returns>
        public static bool HasSource(this AttributeModifierComponent self, long sourceId)
        {
            return self.Modifiers.Values.Any(sources => sources.ContainsKey(sourceId));
        }
        /// <summary>
        /// 清除特定类型的修改器效果
        /// </summary>
        /// <param name="numericType">数值类型</param>
        private static void ClearModifierEffect(this AttributeModifierComponent self, int numericType)
        {
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();
            if (numericComponent == null)
                return;

            // 计算所有修改器的总值
            long totalModifier = 0;
            if (self.Modifiers.TryGetValue(numericType, out var sources))
            {
                foreach (var modifier in sources.Values)
                {
                    totalModifier += modifier;
                }
            }
            if(totalModifier == 0)
                return;

            // 获取当前值
            long currentValue = numericComponent.GetByKey(numericType);
            
            // 清除效果时减去总修改值
            numericComponent.Set(numericType, currentValue - totalModifier);
        }

        /// <summary>
        /// 清除特定来源的所有修改器
        /// </summary>
        /// <param name="sourceId">修改源ID</param>
        public static void ClearSourceModifiers(this AttributeModifierComponent self, long sourceId)
        {
            //如果不存在就return
            if (!self.HasSource(sourceId))
                return;
            
            List<int> affectedTypes = new List<int>();
            
            // 先记录所有要移除的修改器的类型
            foreach (var pair in self.Modifiers)
            {
                if (pair.Value.ContainsKey(sourceId))
                {
                    affectedTypes.Add(pair.Key);
                }
            }
            
            // 对每个类型先清除效果
            foreach (int type in affectedTypes)
            {
                self.ClearModifierEffect(type);
            }

            // 然后移除修改器
            foreach (int type in affectedTypes)
            {
                if (self.Modifiers.TryGetValue(type, out var sources))
                {
                    sources.Remove(sourceId);
                }
            }
        }
        
        /// <summary>
        /// 添加累加修改器
        /// </summary>
        /// <param name="self">修改器组件</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="incrementValue">增量值</param>
        private static void AddAccumulativeModifier(this AttributeModifierComponent self, int numericType, long sourceId, long incrementValue)
        {
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();
            if (numericComponent == null)
                return;
            
            if (!self.Modifiers.TryGetValue(numericType, out var sources))
            {
                sources = new Dictionary<long, long>();
                self.Modifiers[numericType] = sources;
            }
            
            // 获取当前值并累加
            sources.TryGetValue(sourceId, out long currentValue);
            long newValue = currentValue + incrementValue;
            
            //try add sourceId KEY
            if (!sources.TryGetValue(sourceId, out long oldValue))
            {
                sources.Add(sourceId, newValue);
            }
            else
            {
                sources[sourceId] = newValue;
            }
            // 应用修改
            self.ApplyAccumulativeModifier(numericType, incrementValue);
        }
        /// <summary>
        /// 应用累加修改
        /// </summary>
        /// <param name="self">修改器组件</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="incrementValue">增量值</param>
        private static void ApplyAccumulativeModifier(this AttributeModifierComponent self, int numericType, long incrementValue)
        {
            NumericComponent numericComponent = self.GetParent<Unit>().GetComponent<NumericComponent>();
            if (numericComponent == null)
                return;
            long currentValue = numericComponent.GetByKey(numericType);
            // 直接设置为当前值加上修改值
            numericComponent.Set(numericType, currentValue + incrementValue);
        }
    }
}
