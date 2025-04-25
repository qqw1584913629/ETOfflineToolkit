using System;
using System.Collections.Generic;
namespace MH
{
    public enum ModifierType
    {
        Add,
        Override,
    }
    public class AttributeModifierComponent : Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 修改源字典: numericType -> (sourceId -> value)
        /// </summary>
        public Dictionary<int, Dictionary<long, long>> Modifiers = new Dictionary<int, Dictionary<long, long>>();
    }
}
