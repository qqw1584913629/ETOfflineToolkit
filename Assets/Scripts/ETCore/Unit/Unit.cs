using cfg;

namespace MH
{
    public class Unit : Entity, IAwake<int>, ISerializeToEntity
    {
        public UnitType UnitType;
        public int ConfigId;
        public UnitConfig Config => ConfigsSingleton.Instance.Tables.TbUnitConfig.Get(ConfigId);
    }
    public enum UnitType
    {
        Player,
    }
}