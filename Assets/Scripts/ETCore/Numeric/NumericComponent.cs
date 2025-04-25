using System;
using System.Collections.Generic;

namespace MH
{
    [Save]
    [Serializable]
    public class NumericComponent: Entity, IAwake, ISerializeToEntity
    {
        public Dictionary<int, long> NumericDic = new Dictionary<int, long>();
        public long this[int numericType]
        {
            get
            {
                return this.GetByKey(numericType);
            }
            set
            {
                this.Insert(numericType, value);
            }
        }
    }
    public struct NumbericChange
    {
        public Unit Unit;
        public int NumericType;
        public long Old;
        public long New;
    }
}