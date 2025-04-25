using System.Collections.Generic;
using cfg;
using Newtonsoft.Json;
namespace MH
{
    public class BuffInfo : Entity, IAwake, IAwake<int, Unit, Unit>, IDestroy
    {
        public long TimerId;
        
        public long CreateTime;
        public long EndTime;
        public int ConfigId;
        public int Layer;
        public Unit Caster;
        public Unit Target;
        [JsonIgnore]
        public BuffConfig Config => ConfigsSingleton.Instance.Tables.TbBuffConfig.Get(ConfigId);
    }
    public enum BuffAddType
    {
        ResetTime = 1,                     //重置Buff时间
        MultipleLayer,                 //增加Buff层数
        MultipleLayerAndResetTime,     //增加Buff层数且重置Buff时间
    }
    public enum BuffActionType
    {
        Add = 1,
        Sub,
        Override,
    }
    public enum CycleEndType
    {
        Sub = 1,
        Clear,
    }
}