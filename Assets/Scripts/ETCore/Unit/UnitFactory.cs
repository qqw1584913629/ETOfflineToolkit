using cfg;

namespace MH
{
    public static class UnitFactory
    {
        public static async ETTask<Unit> CreateUnit(Scene root, UnitConfig config)
        {
            UnitComponent unitComponent = root.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.GenerateId(), config.Id);
            unit.UnitType = UnitType.Player;
            await EventSystem.Instance.PublishAsync(root, new AfterUnitCreate()
            {
                Unit = unit
            });
            return unit;
        }
    }
}