namespace MH
{
    [EntitySystem]
    public class NumericComponentAwakeSystem : AwakeSystem<NumericComponent>
    {
        protected override void Awake(NumericComponent self)
        {
            
        }
    }

    public static class NumericComponentSystem
    {
        /// <summary>
        /// 获取浮点型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <returns>浮点型数值</returns>   
        public static float GetAsFloat(this NumericComponent self, int numericType)
        {
            return (float)self.GetByKey(numericType) / 10000;
        }
        /// <summary>
        /// 获取整型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <returns>整型数值</returns>    
        public static int GetAsInt(this NumericComponent self, int numericType)
        {
            return (int)self.GetByKey(numericType);
        }
        /// <summary>
        /// 获取长整型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <returns>长整型数值</returns>      
        public static long GetAsLong(this NumericComponent self, int numericType)
        {
            return self.GetByKey(numericType);
        }
        /// <summary>
        /// 设置浮点型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="nt">数值类型</param>
        /// <param name="value">数值</param>    
        public static void Set(this NumericComponent self, int nt, float value)
        {
            self[nt] = (long)(value * 10000);
        }
        /// <summary>
        /// 设置整型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="nt">数值类型</param>
        /// <param name="value">数值</param>    
        public static void Set(this NumericComponent self, int nt, int value)
        {
            self[nt] = value;
        }
        /// <summary>
        /// 设置长整型数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="nt">数值类型</param>
        /// <param name="value">数值</param>        
        public static void Set(this NumericComponent self, int nt, long value)
        {
            self[nt] = value;
        }
        /// <summary>
        /// 设置长整型数值（不触发事件）
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="value">数值</param>    
        public static void SetNoEvent(this NumericComponent self, int numericType, long value)
        {
            self.Insert(numericType, value, false);
        }
        /// <summary>
        /// 插入数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="value">数值</param>
        /// <param name="isPublicEvent">是否触发事件</param>
        public static void Insert(this NumericComponent self, int numericType, long value, bool isPublicEvent = true)
        {
            long oldValue = self.GetByKey(numericType);
            if (oldValue == value)
            {
                return;
            }

            self.NumericDic[numericType] = value;

            if (numericType >= NumericType.Max)
            {
                self.Update(numericType, isPublicEvent);
                return;
            }

            if (isPublicEvent)
            {
                EventSystem.Instance.Publish(self.Root,
                    new NumbericChange() { Unit = self.GetParent<Unit>(), New = value, Old = oldValue, NumericType = numericType });
            }
        }
        /// <summary>
        /// 获取指定键的数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="key">键</param>
        /// <returns>数值</returns>
        public static long GetByKey(this NumericComponent self, int key)
        {
            long value = 0;
            self.NumericDic.TryGetValue(key, out value);
            return value;
        }
        /// <summary>
        /// 更新数值
        /// </summary>
        /// <param name="self">NumericComponent实例</param>
        /// <param name="numericType">数值类型</param>
        /// <param name="isPublicEvent">是否触发事件</param>    
        public static void Update(this NumericComponent self, int numericType, bool isPublicEvent)
        {
            int final = (int)numericType / 10;
            int bas = final * 10 + 1;
            int add = final * 10 + 2;
            int pct = final * 10 + 3;
            int finalAdd = final * 10 + 4;
            int finalPct = final * 10 + 5;

            // 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
            // final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
            long result =
                (long)(((self.GetByKey(bas) + self.GetByKey(add)) * (100 + self.GetAsFloat(pct)) / 100f + self.GetByKey(finalAdd)) *
                    (100 + self.GetAsFloat(finalPct)) / 100f);
            self.Insert(final, result, isPublicEvent);
        }
    }
}